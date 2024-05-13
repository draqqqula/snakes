using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;

namespace SnakeGame.Mechanics.Bodies;

internal abstract class BodyFrame<TShape> : IBodyComponent<TShape> where TShape : IFlatShape
{
    public required TransformBase Transform { get; init; }
    public abstract IEnumerable<TShape> GetBody();
}