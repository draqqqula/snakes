using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Common;
using SnakeGame.Models.Gameplay;

namespace SnakeGame.Services.Gameplay;

internal class MatchManager(MatchConfiguration Configuration) : IUpdateService, IStartUpService, ISessionService
{
    private bool MatchEnded { get; set; } = false;
    private TimeSpan Timer { get; set; } = TimeSpan.Zero;
    private Dictionary<TeamColor, TeamContext> Teams { get; } = [];

    private void AddTeams(params TeamColor[] colors)
    {
        foreach (var color in colors)
        {
            Teams.Add(color, new TeamContext());
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
