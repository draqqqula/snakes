using SnakeGame.Mechanics.Collision.Shapes;

namespace SnakeGame.Mechanics.Collision;

internal interface IFlatShape
{
    public AxisAlignedBoundingBox GetBounds();
    public Polygon AsPolygon();
}
