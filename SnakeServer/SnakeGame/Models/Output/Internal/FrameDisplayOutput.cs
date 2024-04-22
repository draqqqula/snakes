using System.Numerics;

namespace SnakeGame.Models.Output.Internal;

internal struct FrameDisplayOutput
{
    public required Vector2 Position { get; init; }
    public required float Rotation { get; init; }
    public required Vector2 Scale { get; init; }
    public required string Name { get; init; }
}
