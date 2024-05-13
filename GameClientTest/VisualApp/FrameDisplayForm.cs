using Microsoft.Xna.Framework;
using MessageSchemes;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualApp;

public class FrameDisplayForm
{
    public required int Id { get; init; }
    public required Vector2 Position { get; set; }
    public required float Rotation { get; set; }
    public required Vector2 Scale { get; set; }
    public required string Name { get; set; }

    public static FrameDisplayForm FromMessage(Frame frame, string asset)
    {
        return new FrameDisplayForm()
        {
            Name = asset,
            Position = new Vector2(frame.Position.X, frame.Position.Y),
            Scale = new Vector2(frame.Size.X, frame.Size.Y),
            Rotation = frame.Angle,
            Id = frame.Id,
        };
    }
}
