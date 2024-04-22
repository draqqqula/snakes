using MessagePack;
using System.Numerics;

namespace SnakeGame.Models.Output.Internal;

public struct PlayerPositionOutput
{
    public required Vector2 Position { get; init; }
}
