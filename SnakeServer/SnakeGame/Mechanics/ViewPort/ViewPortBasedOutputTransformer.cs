using MessageSchemes;
using ServerEngine.Interfaces.Output;
using SnakeGame.Mechanics.ViewPort;
using SnakeGame.Models.Output.External;

namespace SnakeGame.Mechanics.ViewPorts;

internal class ViewPortBasedOutputTransformer(ViewPortManager Manager) : IOutputCollector<EventMessage>, IOutputProvider<ViewPortBasedBinaryOutput>
{
    public void Pass(EventMessage data)
    {
        foreach (var intersection in Manager.Intersections)
        {
            intersection.Value.Contains(4);
        }
    }

    public ViewPortBasedBinaryOutput Get()
    {
        throw new NotImplementedException();
    }
}
