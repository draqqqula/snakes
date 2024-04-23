using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Output.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class PickupSpawnerService(
    Dictionary<ClientIdentifier, SnakeCharacter> Players, 
    ICollisionChecker CollisionChecker
    ) : 
    IUpdateService, IOutputService<FrameDisplayOutput>
{
    private readonly Dictionary<Guid, TilePickup> _pickups = [];
    private float Time = 0f;

    private const int MaxPickups = 128;

    public IEnumerable<FrameDisplayOutput> Pass()
    {
        foreach ( var tile in _pickups )
        {
            yield return new FrameDisplayOutput()
            {
                Name = $"pickup{tile.Value.Tier}",
                Position = tile.Value.Position,
                Rotation = tile.Value.Rotation,
                Scale = Vector2.One
            };
        }
    }

    public void Update(IGameContext context)
    {
        foreach (var player in Players)
        {
            foreach (var tile in _pickups.ToArray())
            {
                if (CollisionChecker.IsColliding(player.Value, tile.Value))
                {
                    tile.Value.Rotation += context.DeltaTime;
                    _pickups.Remove(tile.Key);
                    player.Value.JoinPart(tile.Value.Tier);
                }
            }
        }

        Time += context.DeltaTime;

        if (Time > 5f)
        {
            for (int i = 0; i < Math.Min(6, MaxPickups-_pickups.Count); i++)
            {
                var tile = new TilePickup()
                {
                    Position = new Vector2(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f) * 500,
                    Rotation = Random.Shared.NextSingle() * MathF.PI,
                    Tier = (byte)Random.Shared.Next(0, 2)
                };
                _pickups.Add(Guid.NewGuid(), tile);
                Time = 0f;
            }
        }
    }
}
