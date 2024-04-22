using System.Numerics;

namespace SnakeGame.Mechanics.Collision.Shapes;

internal record struct AxisAlignedBoundingBox
{
    public Vector2 Min { get; set; }
    public Vector2 Max { get; set; }
}
