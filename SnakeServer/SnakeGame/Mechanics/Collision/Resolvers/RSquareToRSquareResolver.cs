using SnakeGame.Mechanics.Collision.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Mechanics.Collision.Resolvers
{
    internal class RSquareToRSquareResolver
        (ICollisionResolver<Polygon, Polygon> PolygonResolver,
        ICollisionResolver<AxisAlignedBoundingBox, AxisAlignedBoundingBox> AABBResolver)
        : ICollisionResolver<RotatableSquare, RotatableSquare>
    {
        public bool IsColliding(RotatableSquare square1, RotatableSquare square2)
        {
            if (Vector2.Distance(square1.Position, square2.Position) <= square1.Size/2 + square2.Size/2)
            {
                return true;
            }
            if (square1.Rotation % (MathF.PI/2) == 0 && square2.Rotation % (MathF.PI / 2) == 0)
            {
                AABBResolver.IsColliding((AxisAlignedBoundingBox)square1, (AxisAlignedBoundingBox)square2);
            }
            return PolygonResolver.IsColliding((Polygon)square1, (Polygon)square2);
        }
    }
}
