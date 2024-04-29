using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Common;
using SnakeGame.Mechanics.Collision.Resolvers;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Models.Output.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class SnakeSpawner(
    CharacterFabric Fabric,  
    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    List<PickupPoints> Pickups, 
    Dictionary<TeamColor, TeamContext> Teams) : 
    ISessionService, IInputService<MovementDirectionInput>, IOutputService<FrameDisplayOutput>
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
        var team = Teams.Values.Where(it => it.Members.Contains(id)).First();
        var character = Fabric.CreateCharacter();
        character.Position = team.Area.Center + MathEx.AngleToVector(SpawnPositionRandom.NextSingle() * MathF.PI * 2) * (team.Area.Radius / 2);
        character.MovementDirection = MathEx.AngleBetweenVectors(character.Position, Vector2.Zero);
        Players.TryAdd(id, character);
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        var snake = Players[id];
        foreach (var part in snake.Body)
        {
            Pickups.Add(new PickupPoints() 
            { 
                Position = part.Position,
                Rotation = part.Rotation,
                Tier = part.Tier,
            });
        }
        Players.Remove(id);
    }

    public IEnumerable<FrameDisplayOutput> Pass()
    {
        foreach (var character in Players.Values)
        {
            foreach (var bodyPart in character.Body)
            {
                yield return new FrameDisplayOutput()
                {
                    Position = bodyPart.Position,
                    Rotation = bodyPart.Rotation,
                    Name = $"body{bodyPart.Tier}",
                    Scale = System.Numerics.Vector2.One * 4
                };
            }

            yield return new FrameDisplayOutput()
            {
                Name = "head",
                Position = character.Head.Position,
                Rotation = character.Head.Rotation,
                Scale = Vector2.One * 4
            };
        }
    }
}

internal abstract class CharacterFabric
{
    public abstract SnakeCharacter CreateCharacter();
}

internal class CharacterFabricA : CharacterFabric
{
    public override SnakeCharacter CreateCharacter()
    {
        return new SnakeCharacter(Vector2.Zero, 0f, 40f);
    }
}