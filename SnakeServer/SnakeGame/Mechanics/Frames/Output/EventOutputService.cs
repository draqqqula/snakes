using MessageSchemes;
using ServerEngine.Interfaces.Services;

namespace SnakeGame.Mechanics.Frames.Output;

internal class EventOutputService(IMessageProvider Provider) : IOutputService<EventMessage>
{
    public IEnumerable<EventMessage> Pass()
    {
        if (Provider.TryTakeMessage(out var message))
        {
            
            yield return message;
        }
        yield break;
    }
}
