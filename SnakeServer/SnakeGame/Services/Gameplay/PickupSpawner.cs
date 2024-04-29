using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Output.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class PickupSpawner(
    Dictionary<ClientIdentifier, SnakeCharacter> Players, 
    ICollisionChecker CollisionChecker,
    List<PickupPoints> Pickups
    ) : 
    IUpdateService, IOutputService<FrameDisplayOutput>
{
    private float Time = 0f;

    private const int MaxPickups = 128;

    public IEnumerable<FrameDisplayOutput> Pass()
    {
        foreach ( var tile in Pickups )
        {
            yield return new FrameDisplayOutput()
            {
                Name = $"pickup{tile.Tier}",
                Position = tile.Position,
                Rotation = tile.Rotation,
                Scale = Vector2.One * 4
            };
        }
    }

    public void Update(IGameContext context)
    {
        foreach (var player in Players)
        {
            foreach (var tile in Pickups.Where(it => it.Claim != player.Value.Team).ToArray())
            {
                if (CollisionChecker.IsColliding(player.Value, tile))
                {
                    tile.Rotation += context.DeltaTime;
                    Pickups.Remove(tile);
                    player.Value.JoinPart(tile.Tier);
                }
            }
        }

        Time += context.DeltaTime;

        if (Time > 5f)
        {
            for (int i = 0; i < Math.Min(6, MaxPickups-Pickups.Count); i++)
            {
                var tile = new PickupPoints()
                {
                    Position = new Vector2(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f) * 500,
                    Rotation = Random.Shared.NextSingle() * MathF.PI,
                    Tier = (byte)Random.Shared.Next(0, 2)
                };
                Pickups.Add(tile);
                Time = 0f;
            }
        }
    }
}
