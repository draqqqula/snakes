using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal struct SnakeTrailSegment
{
    public required float DistanceTraveled { get; init; }
    public required Vector2 Position { get; init; }
    public required float Rotation { get; init; }
}
