using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using System.Drawing;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal class TilePickup : IBodyComponent<RotatableSquare>
{
    public required float Rotation { get; set; }
    public required Vector2 Position { get; init; }
    public required byte Tier { get; init; }

    public IEnumerable<RotatableSquare> GetBody()
    {
        yield return new RotatableSquare()
        {
            Position = Position,
            Rotation = Rotation,
            Size = 3f
        };
    }
}
