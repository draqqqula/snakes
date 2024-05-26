using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Gameplay;
using SnakeGame.Services.Gameplay.FrameDrivers;

namespace SnakeGame.Services.Gameplay;

internal class SnakeCollisionManager
    (
    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    ICollisionChecker collision,
    List<PickupPoints> Pickups,
    FrameFactory FrameFactory,
    SnakeSpawner SnakeSpawner
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
                snake.Value.Transform.Dispose();
                snake.Value.Head.Transform.Dispose();
                Players[snake.Key] = SnakeSpawner.Spawn(snake.Key);
            }
        }
    }

    public void Interact(SnakeCharacter master, SnakeCharacter slave)
    {
        if (master.Body.Count == 0 || slave.Body.Count == 0)
        {
            return;
        }

        var masterTier = master.MaxTier;
        var slaveTier = slave.MaxTier;

        if (collision.IsColliding(master.Head, slave))
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
            Transform = FrameFactory.Create($"pickup{segment.Tier}", segment.Transform.ReadOnly),
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
            item.Transform.Dispose();
        }
    }
}
