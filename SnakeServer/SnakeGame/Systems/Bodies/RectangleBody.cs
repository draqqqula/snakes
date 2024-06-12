using SnakeGame.Mechanics.Collision.Shapes;

namespace SnakeGame.Mechanics.Bodies;

internal class RectangleBody : BodyFrame<AxisAlignedBoundingBox>
{
    public override IEnumerable<AxisAlignedBoundingBox> GetBody()
    {
        yield return new AxisAlignedBoundingBox()
        {
            Min = Transform.Position - Transform.Size * 0.5f,
            Max = Transform.Position + Transform.Size * 0.5f
        };
    }
}
