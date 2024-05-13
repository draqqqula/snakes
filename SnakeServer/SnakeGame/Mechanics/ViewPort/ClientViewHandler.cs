using ServerEngine.Models;
using SnakeGame.Mechanics.Frames;

namespace SnakeGame.Mechanics.ViewPort;

internal class ClientViewHandler(ViewPortManager Manager)
{
    public required ClientIdentifier Id { get; init; }
    public required EventTable OnScreen { get; set; }
    public required EventTable OutsideScreen { get; set; }

    public void Update(EventTable global)
    {
        var visible = Manager.Intersections[Id];
        var globalDivision = global.Divide((id, entry) => visible.Contains(id));
        var localDivision = OutsideScreen.Divide((id, entry) => visible.Contains(id));

        OnScreen = localDivision.Include.Join(globalDivision.Include);
        OutsideScreen = localDivision.Exclude.Join(globalDivision.Exclude);
    }
}
