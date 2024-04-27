using SyncStates;
using System.Drawing;

namespace SyncTests;

public class MyAggregate
{
    public Point Point { get; init; } = new Point(1, 1);
    public int Integer { get; init; } = 1;
    public string Text { get; init; } = "Default";
    public float Float { get; init; } = 3.1415f;
    public Rectangle Rectangle { get; init; } = new Rectangle(20, 20, 40, 40);
}
