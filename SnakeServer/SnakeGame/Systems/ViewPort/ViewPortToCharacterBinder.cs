using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Models.Gameplay;

namespace SnakeGame.Mechanics.ViewPort;

internal class ViewPortToCharacterBinder(
    Dictionary<ClientIdentifier, ViewPort> ViewPorts,
    Dictionary<ClientIdentifier, SnakeCharacter> Characters) : IUpdateService
{
    public void Update(IGameContext context)
    {
        Bind(context.DeltaTime);
    }

    public void Bind(float deltaTime)
    {
        foreach (var view in ViewPorts)
        {
            if (Characters.TryGetValue(view.Key, out var character))
            {
                var x = MathEx.Lerp(view.Value.Transform.Position.X, character.Transform.Position.X, 0.3f, deltaTime);
                var y = MathEx.Lerp(view.Value.Transform.Position.Y, character.Transform.Position.Y, 0.3f, deltaTime);
                view.Value.Transform.Position = new System.Numerics.Vector2(x, y);
                view.Value.Enabled = true;
            }
        }
    }
}
