using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal class SnakeBodypart : SquareBody
{
    public float DistanceCounter = 0f;
    public Queue<SnakeTrailSegment> Trail { get; } = [];
    public byte Tier { get; set; } = 0;
    public int Value => (int)Math.Pow(2, (Tier+1));
}
