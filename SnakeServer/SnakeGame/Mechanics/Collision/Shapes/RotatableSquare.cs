using System.Collections.Immutable;
using System.Numerics;

namespace SnakeGame.Mechanics.Collision.Shapes;

internal partial record struct RotatableSquare
{
    public required Vector2 Position { get; init; }
    public required float Size { get; init; }
    public required float Rotation { get; init; }
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

    private static RectangleVertexes GetVertexes(RotatableSquare square)
    {
        //Work out the new locations
        var topLeft = RotateVertex(square.Position, square.Size / 2 * new Vector2(-1, -1), square.Rotation);
        var topRight = RotateVertex(square.Position, square.Size / 2 * new Vector2(+1, -1), square.Rotation);
        var bottomLeft = RotateVertex(square.Position, square.Size / 2 * new Vector2(-1, +1), square.Rotation);
        var bottomRight = RotateVertex(square.Position, square.Size / 2 * new Vector2(+1, +1), square.Rotation);
        return new RectangleVertexes()
        {
            TopLeft = topLeft,
            TopRight = topRight,
            BottomLeft = bottomLeft,
            BottomRight = bottomRight
        };
    }

    public static explicit operator Polygon(RotatableSquare square)
    {
        var vertexes = GetVertexes(square);
        Vector2[] vertexArray = 
            [ 
                vertexes.TopRight, 
                vertexes.BottomRight, 
                vertexes.BottomLeft, 
                vertexes.TopLeft 
            ];
        Vector2[] edgeArray =
            [
                vertexes.BottomRight - vertexes.TopRight,
                vertexes.BottomLeft - vertexes.BottomRight,
                vertexes.TopLeft - vertexes.BottomLeft,
                vertexes.TopRight - vertexes.TopLeft
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