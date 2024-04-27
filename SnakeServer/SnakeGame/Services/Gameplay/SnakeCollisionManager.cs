using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Models.Gameplay;

namespace SnakeGame.Services.Gameplay;

internal class SnakeCollisionManager
    (
    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    ICollisionResolver<RotatableSquare, RotatableSquare> collision,
    CharacterFabric fabric,
    Dictionary<Guid, TilePickup> Pickups
    ) : IUpdateService
{
    public void Update(IGameContext context)
    {
        foreach (var player1 in Players.Values)
        {
            if (player1.Body.Count == 0)
            {
                continue;
            }
            foreach (var player2 in Players.Values.Except([player1]))
            {
                if (player2.Body.Count == 0)
                {
                    continue;
                }
                var collapse = false;
                foreach (var part in player1.Body.ToArray())
                {
                    if (collapse)
                    {
                        player1.Body.Remove(part);
                        Pickups.Add(Guid.NewGuid(), new TilePickup() 
                        {
                            Position = part.Position,
                            Rotation = part.Rotation,
                            Tier = part.Tier,
                        });
                        continue;
                    }
                    if (player2.GetBody().Any(it => collision.IsColliding(new RotatableSquare()
                    {
                        Position = part.Position,
                        Rotation = part.Rotation,
                        Size = 4 },
                        it)))
                    {
                        if (part.Tier > player2.Body.Select(it => it.Tier).Max())
                        {
                            foreach (var part2 in player2.Body)
                            {
                                Pickups.Add(Guid.NewGuid(), new TilePickup()
                                {
                                    Position = part.Position,
                                    Rotation = part.Rotation,
                                    Tier = part.Tier,
                                });
                            }
                            player2.Body.Clear();
                        }
                        else
                        {
                            player2.JoinPart(part.Tier);
                            player1.Body.Remove(part);
                            collapse = true;
                        }
                    }
                }
            }
        }
        foreach (var player in Players)
        {
            if (player.Value.Body.Count == 0)
            {
                Players[player.Key] = fabric.CreateCharacter();
            }
        }
    }
}
