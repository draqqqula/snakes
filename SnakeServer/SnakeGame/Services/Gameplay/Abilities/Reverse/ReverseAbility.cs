using SnakeCore.Extensions;
using SnakeGame.Systems.Movement;
using SnakeGame.Systems.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Services.Gameplay.Abilities.Reverse;

internal class ReverseAbility() : CharacterAbility
{
    const bool AllowOrderReverse = true;

    public override float CooldownDuration => 4f;

    protected override void Use(ITimerScheduler scheduler)
    {
        var oldBody = Owner.Body.ToArray();

        TrailNode? transitNode = null;

        foreach (var bodypart in oldBody)
        {
            var reversedTrail = new Trail();
            if (transitNode is not null)
            {
                reversedTrail.ExtendFront(transitNode);
            }

            if (bodypart.Path.Tail is not null)
            {
                var reversedNode = ReverseTrail(bodypart.Path.Tail);
                transitNode = reversedNode;
            }

            bodypart.Path = reversedTrail;

            bodypart.Transform.Angle = ReverseAngle(bodypart.Transform.Angle);
        }
        Owner.Body.Reverse();
        Owner.Transform.Position = Owner.Body.First().Transform.Position;
        Owner.Transform.Angle = Owner.Body.First().Transform.Angle;
        Owner.MovementDirection = Owner.Body.First().Transform.Angle;
    }

    private TrailNode? ReverseTrail(TrailNode end)
    {
        var current = end;
        TrailNode? result = null;
        while (current is not null)
        {
            var reversedAngle = ReverseAngle(current.Rotation);
            var reversedDirection = MathEx.AngleToVector(reversedAngle);
            var newNode = new TrailNode()
            {
                DistanceTraveled = current.DistanceTraveled,
                Position = current.Position + reversedDirection * current.DistanceTraveled,
                Rotation = reversedAngle,
                Next = result
            };
            if (result is not null)
            {
                result.Previous = newNode;
            }
            result = newNode;
            current = current.Next;
        }
        return result;
    }

    private float ReverseAngle(float angle)
    {
        return MathEx.NormalizeAngle(angle + MathF.PI, MathF.PI * 2);
    }
}
