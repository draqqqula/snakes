using ServerEngine.Models;
using SnakeGame.Mechanics.Frames;

namespace SnakeGame.Mechanics.ViewPort;

internal class ClientViewHandler
{
    private EventTable ClientPicture = new EventTable();
    private EventTable ClientInactive = new EventTable();
    public EventTable OnScreen { get; private set; } = new EventTable();
    public EventTable OutsideScreen { get; private set; } = new EventTable();
    public EventTable ToSleep { get; private set; } = new EventTable();

    private List<(EventTable, EventTable)> Log = [];

    public void ApplyEvents(EventTable global, HashSet<int> visible)
    {
        var (globalInclude, globalExclude) = global.Split((id, entry) => visible.Contains(id) || 
        (entry.Lifecycle == EventLifecycle.Dispose && ClientInactive.Contains(id)));

        var (localInclude, localExclude) = OutsideScreen.Split((id, _) => visible.Contains(id));

        // ToSleep = GetSleeping(globalExclude, globalInclude);

        OutsideScreen = localExclude.Join(globalExclude);
        OnScreen = localInclude.Join(globalInclude);

        Log.Add((OutsideScreen, OnScreen));
    }

    private EventTable GetSleeping(EventTable globalExclude, EventTable globalInclude)
    {
        var (fellAsleep, saveState) = ClientPicture.Split((id, _) =>
        {
            return globalExclude.Contains(id) &&
            !ClientInactive.Contains(id);
        });
        var (awaken, continuedSleep) = ClientInactive.Split((id, _) => globalInclude.Contains(id));
        ClientInactive = continuedSleep.Join(fellAsleep);

        return fellAsleep
            .Select(entry => entry.AddAction(EventLifecycle.Sleep));
    }
}
