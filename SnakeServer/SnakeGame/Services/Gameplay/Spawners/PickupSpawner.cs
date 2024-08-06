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
using SnakeGame.Systems.GameObjects.Characters;
using SnakeGame.Systems.Statistics;
using SnakeGame.Systems.Statistics.Interfaces;
using SnakeGameAssets.Services.Interfaces;
using SnakeGameAssets.Services.Models;
using System.Numerics;

namespace SnakeGame.Services.Gameplay.FrameDrivers;

internal class PickupSpawner(

    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    ICollisionChecker CollisionChecker,
    List<PickupPoints> Pickups,
    FrameFactory Factory,
    IPickupPatternContainer PatternContainer,
    IBodyPartFactory BodyPartFactory,
    IRuntimeCommandFactory RuntimeCommandFactory

    ) :

    IUpdateService
{
    private readonly Pattern[] _patterns = PatternContainer.Patterns.ToArray();

    private float Time = 0f;

    private const int MaxPickups = 128;

    private readonly Random _random = new Random();

    public RuntimeCommand<string, Vector2, int> PickupCollected = RuntimeCommandFactory
        .Declare<int>(nameof(PickupCollected))
        .AddArgument<Vector2>()
        .AddArgument<string>()
        .Build();

    private Pattern GetRandomPattern()
    {
        return _patterns[_random.Next(0, _patterns.Length)];
    }

    private void CreateObjects(IEnumerable<Transform> points, Func<byte> tierProvider)
    {
        foreach (var transform in points)
        {
            var tier = tierProvider.Invoke();
            var tile = new PickupPoints()
            {
                Transform = Factory.Create($"pickup{tier}", transform),
                Tier = tier
            };
            Pickups.Add(tile);
        }
    }

    public void Update(IGameContext context)
    {
        foreach (var player in Players)
        {
            foreach (var tile in Pickups.ToArray())
            {
                if (CollisionChecker.IsColliding(player.Value.Head, tile))
                {
                    tile.Transform.Angle += context.DeltaTime;
                    Pickups.Remove(tile);

                    PickupCollected.Call()
                        .Pass("hello world")
                        .Pass(new Vector2(1, 2))
                        .Pass(3)
                        .To(player.Key);

                    player.Value.JoinLast(BodyPartFactory.Create(tile.Transform.ReadOnly, tile.Tier, player.Value.Team));
                    tile.Transform.Dispose();
                }
            }
        }

        Time += context.DeltaTime;

        if (Time > 5f)
        {
            //CreateObjects(Enumerable.Range(0, Math.Min(6, MaxPickups - Pickups.Count)).Select(it => new Transform()
            //{
            //    Angle = _random.NextSingle() * MathF.PI,
            //    Position = new Vector2(_random.NextSingle() - 0.5f, _random.NextSingle() - 0.5f) * 500,
            //    Size = new Vector2(4, 4),
            //}), () => (byte)_random.Next(0, 2));
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
            }
            if (_random.NextSingle() < 0.1f)
            {
                var position = new Vector2(_random.NextSingle() - 0.5f, _random.NextSingle() - 0.5f) * 500;
                var angle = _random.NextSingle() * MathF.PI * 2;
                CreateObjects(GetRandomPattern()
                    .Transformed(angle, 4f, position)
                    .Select(it => new Transform() 
                    { 
                        Angle = angle, 
                        Position = it, 
                        Size = Vector2.One * 4f 
                    }), () => 0);
            }
            Time = 0f;
        }
    }
}
