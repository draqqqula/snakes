using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Services.Output;
using SnakeGame.Services.Output.Commands;

namespace SnakeGame.Mechanics.ViewPort.Display;

internal class ViewPortDisplayHelper(Dictionary<ClientIdentifier, ViewPort> ViewPorts, FrameFactory Factory) : IUpdateService, IOutputService<ClientCommandWrapper>
{
    private readonly Dictionary<ViewPort, ViewDisplay> _displayObjects = [];

    private readonly List<ClientCommandWrapper> Commands = [];

    public IEnumerable<ClientCommandWrapper> Pass()
    {
        foreach(var command in Commands)
        {
            yield return command;
        }
        Commands.Clear();
    }

    public void Update(IGameContext context)
    {
        foreach (var viewPort in ViewPorts)
        {
            ViewDisplay display;
            if (_displayObjects.TryGetValue(viewPort.Value, out display))
            {
                display.Transform.Position = viewPort.Value.Transform.Position;
                display.Transform.Angle = viewPort.Value.Transform.Angle;
                display.Transform.Size = viewPort.Value.Transform.Size;
            }
            else
            {
                display = new ViewDisplay()
                {
                    Transform = Factory.Create("viewport", viewPort.Value.Transform.ReadOnly),
                };
                Commands.Add(new ClientCommandWrapper()
                {
                    Id = viewPort.Key,
                    Command = new AttachCameraCommand()
                    {
                        FrameId = display.Transform.Id.Value
                    }
                });
                _displayObjects.Add(viewPort.Value, display);
            }
        }
    }
}
