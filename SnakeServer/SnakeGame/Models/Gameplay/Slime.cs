using SnakeGame.Common;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Physics;

namespace SnakeGame.Models.Gameplay;

internal class Slime : PickupPoints
{
    public void UpdateStatus(float deltaTime)
    {
        StunDuration = MathF.Max(0, StunDuration - deltaTime);
    }
    public void Stun(float duration)
    {
        StunDuration = duration;
    }
    public float StunDuration { get; private set; }
    public bool Stunned => StunDuration > 0;
    public required TeamContext Team { get; init; }
}
