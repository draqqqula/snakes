using ServerEngine.Models;
using SnakeGame.Mechanics.Frames;

namespace SnakeGame.Mechanics.ViewPort;

internal readonly struct ClientEventSet
{
    public required ClientIdentifier Id { get; init; }
    public required EventTable Table { get; init; }
}
