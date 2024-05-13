using MessageSchemes;

namespace SnakeGame.Mechanics.Frames;

internal interface IMessageProvider
{
    public bool TryTakeMessage(out EventMessage? message);
}
