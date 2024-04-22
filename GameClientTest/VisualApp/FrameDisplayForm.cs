using Microsoft.Xna.Framework;

namespace VisualApp;

public struct FrameDisplayForm
{
    public required Vector2 Position { get; init; }
    public required float Rotation { get; init; }
    public required Vector2 Scale { get; init; }
    public required string Name { get; init; }
}
