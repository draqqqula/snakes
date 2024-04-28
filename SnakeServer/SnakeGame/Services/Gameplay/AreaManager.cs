using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using SnakeGame.Common;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Output.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class AreaManager(Dictionary<TeamColor, TeamContext> Teams) : IUpdateService, IOutputService<FrameDisplayOutput>
{
    public IEnumerable<FrameDisplayOutput> Pass()
    {
        foreach (var team in Teams.Values.Select(it => it.Area))
        {
            yield return new FrameDisplayOutput()
            {
                Name = "area",
                Position = team.Center,
                Rotation = 0f,
                Scale = Vector2.One * (1/100) * team.Radius
            };
        }
    }

    public void Update(IGameContext context)
    {
        
    }
}
