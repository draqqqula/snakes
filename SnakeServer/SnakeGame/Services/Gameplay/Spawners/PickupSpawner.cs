using MessageSchemes;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Output.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay.FrameDrivers;

internal class PickupSpawner(
    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    ICollisionChecker CollisionChecker,
    List<PickupPoints> Pickups,
    FrameFactory Factory
    ) :
    IUpdateService
{
    private float Time = 0f;

    private const int MaxPickups = 128;

    private readonly Random _random = new Random();

    public void Update(IGameContext context)
    {
        foreach (var player in Players)
        {
            foreach (var tile in Pickups.Where(it => it.Claim != player.Value.Team).ToArray())
            {
                if (CollisionChecker.IsColliding(player.Value.Head, tile))
                {
                    tile.Transform.Angle += context.DeltaTime;
                    Pickups.Remove(tile);
                    player.Value.JoinPart(tile.Tier, Factory);
                    tile.Transform.Dispose();
                }
            }
        }

        Time += context.DeltaTime;

        if (Time > 5f)
        {
            for (int i = 0; i < Math.Min(6, MaxPickups - Pickups.Count); i++)
            {
                var tier = (byte)_random.Next(0, 2);
                var tile = new PickupPoints()
                { 
                    Transform = Factory.Create($"pickup{tier}", new Transform()
                    {
                        Angle = _random.NextSingle() * MathF.PI,
                        Size = new Vector2(4, 4),
                        Position = new Vector2(_random.NextSingle() - 0.5f, _random.NextSingle() - 0.5f) * 500
                    }),
                    Tier = tier
                };
                Pickups.Add(tile);
                Time = 0f;
            }
        }
    }
}
