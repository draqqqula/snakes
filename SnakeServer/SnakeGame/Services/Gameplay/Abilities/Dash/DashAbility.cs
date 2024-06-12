using SnakeGame.Models.Gameplay;
using SnakeGame.Systems.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Services.Gameplay.Abilities.Dash;

internal class DashAbility : CharacterAbility
{
    public override float CooldownDuration => 5f;

    protected override void Use(ITimerScheduler scheduler)
    {
        Owner.Speed = 120f;
        Owner.BodyIndentation = 8f;
        scheduler.SetSeconds(0.2f, () => CancelDash(Owner));
    }

    private static void CancelDash(SnakeCharacter character)
    {
        character.BodyIndentation = 4f;
        character.Speed = 40f;
    }
}
