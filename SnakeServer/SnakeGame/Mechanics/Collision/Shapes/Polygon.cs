using System.Collections.Immutable;
using System.Numerics;

namespace SnakeGame.Mechanics.Collision.Shapes;

internal record struct Polygon
{
    public ImmutableArray<Vector2> Vertexes { get; init; }
    public ImmutableArray<Vector2> Edges { get; init; }
}
