using SnakeGame.Systems.GameObjects.Characters;
using SnakeGame.Systems.Respawn;

namespace SnakeGame.Services.Gameplay.Abilities;

internal class AbilityFactory<T> : IAbilityFactory where T : CharacterAbility, new()
{
    public CharacterAbility Create(SnakeCharacter character)
    {
        return new T()
        { 
            Owner = character 
        };
    }
}
