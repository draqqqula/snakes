using ServerEngine.Models;
using SnakeGame.Mechanics.Frames;

namespace SnakeGame.Mechanics.ViewPort;

internal class ClientViewHandler
{
    private EventTable ClientActive = new EventTable();
    private EventTable ClientState = new EventTable();
    public EventTable OnScreen { get; private set; } = new EventTable();
    public EventTable OutsideScreen { get; private set; } = new EventTable();

    public void ApplyEvents(EventTable global, HashSet<int> visible)
    {
        var (globalInclude, globalExclude) = global.Split((id, entry) => visible.Contains(id) || 
        (entry.Lifecycle == EventLifecycle.Dispose && ClientState.Contains(id)));

        var (localInclude, localExclude) = OutsideScreen.Split((id, _) => visible.Contains(id));

        OutsideScreen = localExclude.Join(globalExclude);
        OnScreen = localInclude.Join(globalInclude);

        // если вносятся изменения в блок, который сейчас отгружен на клиенте, то его состояние помечается как сон
        ApplySleep(global, visible);
    }

    private void ApplySleep(EventTable global, HashSet<int> visible)
    {
        var activeState = ClientActive.Join(OnScreen);
        ClientState = ClientState.Join(OnScreen);

        var (loaded, unloaded) = activeState.Split((id, _) => visible.Contains(id));
        var (changed, unchanged) = unloaded.Split((id, _) =>
        { 
            if (global.Contains(id))
            {
                OnScreen[id].AddAction(EventLifecycle.Sleep);
                return true;
            }
            return false; 
        });

        ClientActive = loaded.Join(unchanged);
    }
}
