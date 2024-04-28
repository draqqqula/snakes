using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal class TeamArea : IBodyComponent<Circle>
{
    public float Radius { get; set; } = 15f;
    public float ConsumeSpeed { get; set; } = 1f;
    public Vector2 Center { get; init; }
    public IEnumerable<Circle> GetBody()
    {
        yield return new Circle()
        {
            Position = Center,
            Radius = Radius
        };
    }
}
