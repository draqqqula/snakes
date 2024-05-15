using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Frames;

namespace SnakeGame.Mechanics.ViewPort.Display;

internal class ViewPortDisplayHelper(Dictionary<ClientIdentifier, ViewPort> ViewPorts, FrameFactory Factory) : IUpdateService
{
    private readonly Dictionary<ViewPort, ViewDisplay> _displayObjects = [];
    public void Update(IGameContext context)
    {
        foreach (var viewPort in ViewPorts.Values)
        {
            ViewDisplay display;
            if (_displayObjects.TryGetValue(viewPort, out display))
            {
                display.Transform.Position = viewPort.Transform.Position;
                display.Transform.Angle = viewPort.Transform.Angle;
                display.Transform.Size = viewPort.Transform.Size;
            }
            else
            {
                display = new ViewDisplay()
                {
                    Transform = Factory.Create("viewport", viewPort.Transform.ReadOnly),
                };
                _displayObjects.Add(viewPort, display);
            }
        }
    }
}
