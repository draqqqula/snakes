using SnakeGame.Common;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using System.Drawing;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal class PickupPoints : SquareBody
{
    public TeamColor? Claim { get; set; }
    public required byte Tier { get; set; }
    public int Value => (int)Math.Pow(2, (Tier + 1));
}
