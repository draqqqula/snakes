using SnakeGame.Mechanics.Collision.Shapes;

namespace SnakeGame.Mechanics.Collision.Resolvers;

internal class AABBToAABBResolver : ICollisionResolver<AxisAlignedBoundingBox, AxisAlignedBoundingBox>
{
    public bool IsColliding(AxisAlignedBoundingBox aabb1, AxisAlignedBoundingBox aabb2)
    {
        return aabb1.Min.X <= aabb2.Max.X &&
                   aabb1.Max.X >= aabb2.Min.X &&
                   aabb1.Min.Y <= aabb2.Max.Y &&
                   aabb1.Max.Y >= aabb2.Min.Y;
    }
}
