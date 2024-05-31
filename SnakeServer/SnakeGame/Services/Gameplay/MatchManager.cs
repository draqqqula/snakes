using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Common;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Gameplay;
using SnakeGame.Services.Output.Commands;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class MatchManager(
    MatchConfiguration Configuration, 
    Dictionary<TeamColor, TeamContext> Teams, 
    FrameFactory Factory,
    CommandSender Sender,
    IInternalSessionController InternalController
    ) : 
    IUpdateService, IStartUpService, ISessionService
{
    private const float AreaDistance = 200;
    private readonly Vector2[] Locations = 
        [ 
            new Vector2(1, 1) * AreaDistance,
            new Vector2(-1, -1) * AreaDistance,
            new Vector2(1, -1) * AreaDistance,
            new Vector2(-1, 1) * AreaDistance
        ];


    private const float GraceTime = 5;
    private bool OnGrace { get; set; } = false;
    private bool MatchEnded { get; set; } = false;
    private TimeSpan Timer { get; set; } = TimeSpan.Zero;

    private void AddTeams(params TeamColor[] colors)
    {
        foreach (var (color, index) in colors.Select((it, i) => (it, i)))
        {
            var area = new TeamArea()
            {
                Transform = Factory.Create($"area_{color}", new Transform()
                {
                    Angle = 0f,
                    Position = Locations[index],
                    Size = Vector2.One * 40f
                }),
            };
            Teams.Add(color, new TeamContext(area));
        }
    }

    public void Start(IGameContext context)
    {
        Timer = Configuration.Duration;
        if (Configuration.Mode == GameMode.Dual)
        {
            AddTeams(TeamColor.Red, TeamColor.Blue);
        }
        else if (Configuration.Mode == GameMode.Quad)
        {
            AddTeams(TeamColor.Red, TeamColor.Blue, TeamColor.Yellow, TeamColor.Green);
        }
    }

    public void Update(IGameContext context)
    {
        Timer -= TimeSpan.FromSeconds((double)context.DeltaTime);

        if (Timer <= TimeSpan.Zero)
        {
            if (OnGrace)
            {
                MatchEnded = true;
                InternalController.Finish();
            }
            else
            {
                InternalController.SetTimeScale(0.5f);
                Timer = TimeSpan.FromSeconds(GraceTime);
                OnGrace = true;
            }
        }
    }

    public void OnJoin(IGameContext context, ClientIdentifier id)
    {
        var team = Teams.Values
            .Where(it => it.Members.Count < Configuration.TeamSize)
            .OrderBy(it => it.Members.Count)
            .FirstOrDefault();
        if (team is null)
        {
            return;
        }
        team.Members.Add(id);
        UpdateTimerCommand.To(id, Sender, Timer);
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        foreach(var team in Teams)
        {
            team.Value.Members.Remove(id);
        }
    }
}
