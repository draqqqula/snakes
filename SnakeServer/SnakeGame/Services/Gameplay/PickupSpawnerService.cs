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
    ICollisionResolver<RotatableSquare, RotatableSquare> CollisionResolver
    ) : 
    IUpdateService, IOutputService<FrameDisplayOutput>
{
    private readonly Dictionary<Guid, TilePickup> _tiles = [];
    private float Time = 0f;

    public IEnumerable<FrameDisplayOutput> Pass()
    {
        foreach ( var tile in _tiles )
        {
            yield return new FrameDisplayOutput()
            {
                Name = "Pickup",
                Position = tile.Value.Position,
                Rotation = tile.Value.Rotation,
                Scale = System.Numerics.Vector2.One
            };
        }
    }

    public void Update(IGameContext context)
    {
        foreach (var player in Players)
        {
            foreach (var tile in _tiles.ToArray())
            {
                if (CollisionResolver.IsColliding(player.Value.GetBody().First(), tile.Value.GetBody().First()))
                {
                    tile.Value.Rotation += context.DeltaTime;
                    _tiles.Remove(tile.Key);
                }
            }
        }

        Time += context.DeltaTime;

        if (Time > 5f)
        {
            for (int i = 0; i < 6; i++)
            {
                var tile = new TilePickup()
                {
                    Position = new Vector2(Random.Shared.NextSingle(), Random.Shared.NextSingle()) * 500,
                    Rotation = Random.Shared.NextSingle() * MathF.PI,
                    Value = 2
                };
                _tiles.Add(Guid.NewGuid(), tile);
                Time = 0f;
            }
        }
    }
}
