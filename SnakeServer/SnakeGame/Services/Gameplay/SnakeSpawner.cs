using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Collision.Resolvers;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Models.Output.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class SnakeSpawner(
    CharacterFabric Fabric,  
    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    Dictionary<Guid, TilePickup> Pickups) : 
    ISessionService, IInputService<MovementDirectionInput>, IOutputService<FrameDisplayOutput>
{

    public void OnInput(ClientIdentifier id, MovementDirectionInput data)
    {
        if (Players.TryGetValue(id, out SnakeCharacter character))
        {
            character.MovementDirection = data.Angle;
        }
    }

    public void OnJoin(IGameContext context, ClientIdentifier id)
    {
        var character = Fabric.CreateCharacter();
        Players.TryAdd(id, character);
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        var snake = Players[id];
        foreach (var part in snake.Body)
        {
            Pickups.Add(Guid.NewGuid(), new TilePickup() 
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
                    Scale = System.Numerics.Vector2.One
                };
            }

            yield return new FrameDisplayOutput()
            {
                Name = "head",
                Position = character.Head.Position,
                Rotation = character.Head.Rotation,
                Scale = Vector2.One
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