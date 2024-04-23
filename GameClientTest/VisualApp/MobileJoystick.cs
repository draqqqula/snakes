using Microsoft.Xna.Framework;

namespace VisualApp;

public class MobileJoystick
{
    public float Direction { get; set; }
    public bool Active { get; set; } = false;
    public Vector2 ScreenPosition { get; set; } = Vector2.Zero;
    public Vector2 DotPosition { get; set; } = Vector2.Zero;
}
