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
    List<PickupPoints> Pickups
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
        var slaveTier = slave.Body.Select(it => it.Tier).Max();

        if (collision.IsColliding(master, slave))
        {
            Console.WriteLine($"Head Collision: master - {master} slave - {slave}");
            if (masterTier >= slaveTier)
            {
                Console.WriteLine($"Master tier({masterTier}) greater or equal slave tier({slaveTier})");
                CollapseBody(slave, 0);
            }
            if (slaveTier >= masterTier)
            {
                Console.WriteLine($"Slave tier({masterTier}) greater or equal master tier({slaveTier})");
                CollapseBody(master, 0);
            }
            Console.WriteLine($"Result: master - {master} slave - {slave}");
            return;
        }

        foreach (var (segment, index) in slave.Body.Select((i, it) => (i, it)))
        {
            if (!collision.IsColliding(master, segment))
            {
                continue;
            }

            Console.WriteLine($"Segment collision: master - {master} slave - {slave} segment index {index}");

            if (masterTier <= segment.Tier)
            {
                Console.WriteLine($"masterTier less or equal segmentTier");
                CollapseBody(master, 0);
            }
            if (masterTier >= segment.Tier)
            {
                Console.WriteLine($"masterTier greater or equal segmentTier");
                CollapseBody(slave, index);
            }
            Console.WriteLine($"Result: master - {master} slave - {slave}");
            return;
        }
    }

    private void SpawnPickup(SnakeBodypart segment)
    {
        Pickups.Add(new PickupPoints()
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
        var collapsed = character.Body.Skip(start).ToArray();
        foreach ( var item in collapsed)
        {
            SpawnPickup(item);
            character.Body.Remove(item);
        }
    }
}
