using MessageSchemes;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Common;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision.Resolvers;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Models.Output.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay.FrameDrivers;

internal class SnakeSpawner(
    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    List<PickupPoints> Pickups,
    Dictionary<TeamColor, TeamContext> Teams,
    FrameFactory FrameFactory
    ) :
    ISessionService, IInputService<MovementDirectionInput>
{
    private Random SpawnPositionRandom { get; } = new Random();
    public void OnInput(ClientIdentifier id, MovementDirectionInput data)
    {
        if (Players.TryGetValue(id, out SnakeCharacter character))
        {
            character.MovementDirection = data.Angle;
        }
    }

    public void OnJoin(IGameContext context, ClientIdentifier id)
    {
        var character = Spawn(id);
        Players.TryAdd(id, character);
    }

    public SnakeCharacter Spawn(ClientIdentifier id)
    {
        var team = Teams.Where(it => it.Value.Members.Contains(id)).First();
        var position = 
            team.Value.Area.Transform.Position + 
            MathEx.AngleToVector(SpawnPositionRandom.NextSingle() * MathF.PI * 2) * 
            (team.Value.Area.Radius / 2);
        var angle = MathEx.AngleBetweenVectors(position, Vector2.Zero);

        var transform = new Transform()
        {
            Angle = angle,
            Position = position,
            Size = new Vector2(4, 4),
        };

        var main = new TransformObject()
        {
            Angle = transform.Angle,
            Position = position,
            Size = transform.Size,
        };
        var head = new Mechanics.Bodies.SquareBody() 
        { 
            Transform = FrameFactory.Create("head", transform) 
        };
        var body = new SnakeBodypart()
        {
            Transform = FrameFactory.Create("body0", transform),
            Tier = 0,
        };

        var character = new SnakeCharacter()
        {
            Transform = main,
            Head = head,
            Body = [ body ],
            MovementDirection = angle,
            Team = team.Key
        };
        return character;
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        var snake = Players[id];
        foreach (var segment in snake.Body)
        {
            Pickups.Add(new PickupPoints()
            {
                Transform = FrameFactory.Create($"pickup{segment.Tier}", segment.Transform.ReadOnly),
                Tier = segment.Tier,
            });
            segment.Transform.Dispose();
        }
        Players.Remove(id);
        snake.Head.Transform.Dispose();
        snake.Transform.Dispose();
    }
}