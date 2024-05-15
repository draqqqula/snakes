using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Mechanics.Frames.Output.Interfaces;

namespace SnakeGame.Mechanics.ViewPort.Output;

internal class ViewOutputService(ViewPortManager Manager, FrameStorage Storage, ITableProvider Provider) : 
    IOutputService<ClientEventSet>
{
    private readonly Dictionary<ClientIdentifier, ClientViewHandler> _views = [];

    public IEnumerable<ClientEventSet> Pass()
    {
        var globalChanges = Provider.Take();
        var state = Storage.GetAll();

        foreach (var viewSet in Manager.ActiveIntersections)
        {
            var hashSet = viewSet.Value.ToHashSet();
            ClientViewHandler? handler;

            if (_views.TryGetValue(viewSet.Key, out handler))
            {
                handler.ApplyEvents(globalChanges, hashSet);
            }
            else
            {
                handler = new ClientViewHandler();
                handler.ApplyEvents(EventTable.FromState(state), hashSet);
                _views.Add(viewSet.Key, handler);
            }

            yield return new ClientEventSet()
            {
                Id = viewSet.Key,
                Table = handler.OnScreen
            };
        }
    }
}
