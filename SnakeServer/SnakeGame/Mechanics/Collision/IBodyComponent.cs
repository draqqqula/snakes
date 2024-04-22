namespace SnakeGame.Mechanics.Collision;

internal interface IBodyComponent<TShape>
{
    public IEnumerable<TShape> GetBody();
}
