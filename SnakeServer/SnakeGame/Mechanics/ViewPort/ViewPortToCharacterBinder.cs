using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Models.Gameplay;

namespace SnakeGame.Mechanics.ViewPort;

internal class ViewPortToCharacterBinder(
    Dictionary<ClientIdentifier, ViewPort> ViewPorts,
    Dictionary<ClientIdentifier, SnakeCharacter> Characters) : IUpdateService
{
    public void Update(IGameContext context)
    {
        foreach (var view in ViewPorts)
        {
            if (Characters.TryGetValue(view.Key, out var character))
            {
                view.Value.Transform.Position = character.Transform.Position;
            }
        }
    }
}
