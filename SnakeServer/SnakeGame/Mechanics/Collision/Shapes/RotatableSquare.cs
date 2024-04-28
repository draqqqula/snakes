using System.Collections.Immutable;
using System.Numerics;

namespace SnakeGame.Mechanics.Collision.Shapes;

internal partial record struct RotatableSquare
{
    public required Vector2 Position { get; init; }
    public required float Size { get; init; }
    public required float Rotation { get; init; }
    public readonly Vector2 TopLeft
    {
        get
        {
            return RotateVertex(Position, Size / 2 * new Vector2(-1, -1), Rotation);
        }
    }
    public readonly Vector2 TopRight
    {
        get
        {
            return RotateVertex(Position, Size / 2 * new Vector2(+1, -1), Rotation);
        }
    }
    public readonly Vector2 BottomLeft
    {
        get
        {
            return RotateVertex(Position, Size / 2 * new Vector2(-1, +1), Rotation);
        }
    }
    public readonly Vector2 BottomRight
    {
        get
        {
            return RotateVertex(Position, Size / 2 * new Vector2(+1, +1), Rotation);
        }
    }

    public readonly float DiagonalLength
    {
        get
        {
            return MathF.Sqrt(MathF.Pow(Size, 2) + MathF.Pow(Size, 2));
        }
    }
}

internal partial record struct RotatableSquare
{
    struct RectangleVertexes
    {
        public Vector2 TopLeft;
        public Vector2 TopRight;
        public Vector2 BottomLeft;
        public Vector2 BottomRight;
    }

    private static Vector2 RotateVertex(Vector2 center, Vector2 delta, float rotation)
    {
        float distance = delta.Length();
        var originalAngle = MathF.Atan2(delta.Y, delta.X);
        float rotatedX = center.X + distance * MathF.Cos(originalAngle + rotation);
        float rotatedY = center.Y + distance * MathF.Sin(originalAngle + rotation);

        return new Vector2(rotatedX, rotatedY);
    }

    public static explicit operator Polygon(RotatableSquare square)
    {
        Vector2[] vertexArray = 
            [ 
                square.TopRight, 
                square.BottomRight, 
                square.BottomLeft, 
                square.TopLeft 
            ];
        Vector2[] edgeArray =
            [
                square.BottomRight - square.TopRight,
                square.BottomLeft - square.BottomRight,
                square.TopLeft - square.BottomLeft,
                square.TopRight - square.TopLeft
            ];
        return new Polygon() 
        { 
            Edges = edgeArray.ToImmutableArray(), 
            Vertexes = vertexArray.ToImmutableArray()
        };
    }
}

internal partial record struct RotatableSquare
{
    public static explicit operator AxisAlignedBoundingBox(RotatableSquare square)
    {
        return new AxisAlignedBoundingBox()
        {
            Min = square.Position - Vector2.One * square.Size,
            Max = square.Position + Vector2.One * square.Size
        };
    }
}