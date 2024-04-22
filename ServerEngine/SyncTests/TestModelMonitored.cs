using SyncStates;
using System.Drawing;

namespace SyncTests;

public class TestModelMonitored
{
    public Monitored<Point> Point { get; init; } = new Point(1, 1);
    public Monitored<int> Integer { get; init; } = 1;
    public Monitored<string> Text { get; init; } = "Default";
    public Monitored<float> Float { get; init; } = 3.1415f;
    public Monitored<Rectangle> Rectangle { get; init; } = new Rectangle(20, 20, 40, 40);
}
