using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal class SnakeBodypart
{
    public float DistanceCounter = 0f;
    public Queue<SnakeTrailSegment> Trail { get; } = [];
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public byte Tier { get; set; } = 0;
    public int Value => (int)Math.Pow(2, (Tier+1));
}
