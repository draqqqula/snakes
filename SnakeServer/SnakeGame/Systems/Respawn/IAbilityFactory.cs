using SnakeGame.Models.Gameplay;
using SnakeGame.Services.Gameplay.Abilities;

namespace SnakeGame.Systems.Respawn;

internal interface IAbilityFactory
{
    public CharacterAbility Create(SnakeCharacter character);
}
