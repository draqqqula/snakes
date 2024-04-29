using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Common;
using SnakeGame.Models.Gameplay;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class MatchManager(MatchConfiguration Configuration, Dictionary<TeamColor, TeamContext> Teams) : IUpdateService, IStartUpService, ISessionService
{
    private const float AreaDistance = 200;
    private readonly Vector2[] Locations = 
        [ 
            new Vector2(1, 1) * AreaDistance,
            new Vector2(-1, -1) * AreaDistance,
            new Vector2(1, -1) * AreaDistance,
            new Vector2(-1, 1) * AreaDistance
        ];

    private bool MatchEnded { get; set; } = false;
    private TimeSpan Timer { get; set; } = TimeSpan.Zero;

    private void AddTeams(params TeamColor[] colors)
    {
        foreach (var (color, index) in colors.Select((it, i) => (it, i)))
        {
            var area = new TeamArea()
            {
                Center = Locations[index]
            };
            Teams.Add(color, new TeamContext(area));
        }
    }

    public void Start(IGameContext context)
    {
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
        if (MatchEnded)
        {
            return;
        }
        Timer += TimeSpan.FromSeconds((double)context.DeltaTime);
        if (Timer > Configuration.Duration)
        {
            MatchEnded = true;
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
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        foreach(var team in Teams)
        {
            team.Value.Members.Remove(id);
        }
    }
}
