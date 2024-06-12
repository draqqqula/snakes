using SnakeCore.Extensions;
using System.Numerics;

namespace SnakeGame.Systems.Movement;

internal record class TrailSegment
{
    public required float DistanceTraveled { get; set; }
    public required Vector2 Position { get; set; }
    public required float Rotation { get; set; }

    public bool TryCut(float distance, out TrailSegment rest)
    {
        var delta = distance - DistanceTraveled;
        if (delta >= 0)
        {
            rest = this;
            return false;
        }

        rest = this with
        {
            DistanceTraveled = distance,
            Position = Position - MathEx.AngleToVector(Rotation) * (DistanceTraveled - distance)
        };
        DistanceTraveled -= distance;
        return true;
    }
}
