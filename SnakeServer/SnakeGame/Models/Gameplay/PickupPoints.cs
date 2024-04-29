using SnakeGame.Common;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using System.Drawing;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal class PickupPoints : IBodyComponent<RotatableSquare>
{
    public TeamColor? Claim { get; set; }
    public required float Rotation { get; set; }
    public required Vector2 Position { get; set; }
    public required byte Tier { get; set; }
    public int Value => (int)Math.Pow(2, (Tier + 1));

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
