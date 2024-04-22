using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Collision.Resolvers;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Models.Output.Internal;

namespace SnakeGame.Services.Gameplay;

internal class PlayerSpawnerService(
    CharacterFabric Fabric,  
    Dictionary<ClientIdentifier, SnakeCharacter> Players) : 
    ISessionService, IInputService<MovementDirectionInput>, IUpdateService, IOutputService<FrameDisplayOutput>
{

    public void OnInput(ClientIdentifier id, MovementDirectionInput data)
    {
        if (Players.TryGetValue(id, out SnakeCharacter character))
        {
            character.Direction = data.Direction;
        }
    }

    public void OnJoin(IGameContext context, ClientIdentifier id)
    {
        var character = Fabric.CreateCharacter();
        Players.TryAdd(id, character);
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        Players.Remove(id);
    }

    public void Update(IGameContext context)
    {
        foreach (var character in Players.Values)
        {
            character.Position += character.Speed * character.Direction.ToVector2() * context.DeltaTime;
            character.GetBody();
        }
    }

    public IEnumerable<FrameDisplayOutput> Pass()
    {
        foreach (var character in Players.Values)
        {
            yield return new FrameDisplayOutput()
            {
                Position = character.Position,
                Rotation = character.Rotation,
                Name = "Player",
                Scale = System.Numerics.Vector2.One
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
        return new SnakeCharacter()
        {
            Position = System.Numerics.Vector2.One,
            Speed = 40,
            Direction = MovementDirection.Left,
            Rotation = 0
        };
    }
}