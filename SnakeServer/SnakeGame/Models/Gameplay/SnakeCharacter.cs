using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Models.Input.Internal;
using System.Drawing;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal record SnakeCharacter : IBodyComponent<RotatableSquare>
{
    public const float Size = 4;
    public required MovementDirection Direction { get; set; } = MovementDirection.Left;
    public required Vector2 Position { get; set; } = Vector2.Zero;
    public required float Rotation { get; set; } = 0f;
    public required float Speed { get; set; } = 0.05f;
    public IEnumerable<RotatableSquare> GetBody()
    {
        yield return new RotatableSquare()
        {
            Position = Position,
            Rotation = Rotation,
            Size = Size,
        };
    }
}
