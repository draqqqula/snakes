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
    ICollisionChecker collision,
    CharacterFabric fabric,
    Dictionary<Guid, TilePickup> Pickups
    ) : IUpdateService
{
    public void Update(IGameContext context)
    {
        foreach (var master in Players.Values)
        {
            foreach (var slave in Players.Values.Except([master]))
            {
                Interact(master, slave);
            }
        }
        foreach (var snake in Players)
        {
            if (snake.Value.Body.Count == 0)
            {
                Players[snake.Key] = fabric.CreateCharacter();
            }
        }
    }

    public void Interact(SnakeCharacter master, SnakeCharacter slave)
    {
        if (master.Body.Count == 0 || slave.Body.Count == 0)
        {
            return;
        }

        var masterTier = master.Body.Select(it => it.Tier).Max();
        foreach (var (segment, index) in slave.Body.Select((i, it) => (i, it)))
        {
            if (!collision.IsColliding(master, segment))
            {
                continue;
            }

            if (masterTier >= segment.Tier)
            {
                CollapseBody(master, 0);
                return;
            }
            if (masterTier <= segment.Tier)
            {
                CollapseBody(slave, index);
                return;
            }
        }
    }

    private void SpawnPickup(SnakeBodypart segment)
    {
        Pickups.Add(Guid.NewGuid(), new TilePickup()
        {
            Position = segment.Position,
            Rotation = segment.Rotation,
            Tier = segment.Tier,
        });
    }

    private void CollapseBody(SnakeCharacter character, int start)
    {
        if (start >= character.Body.Count)
        {
            return;
        }
        var collapsed = character.Body.Skip(start);
        foreach ( var item in collapsed)
        {
            SpawnPickup(item);
            character.Body.Remove(item);
        }
    }
}
