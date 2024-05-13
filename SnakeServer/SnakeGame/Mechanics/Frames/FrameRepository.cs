using MessageSchemes;
using SnakeGame.Mechanics.Bodies;
using System.Numerics;

namespace SnakeGame.Mechanics.Frames;

internal class FrameRepository : INotificationListener, IMessageProvider
{
    protected EventTable Table { get; set; } = new EventTable();

    public void NotifyCreated(int id, string asset, Transform transform)
    {
        var row = Table[id];
        row.SetAction(EventLifecycle.Create);
        row.Asset = asset;
        row.Position = transform.Position;
        row.Size = transform.Size;
        row.Angle = transform.Angle;
    }

    public void NotifyDisposed(int id)
    {
        Table[id].SetAction(EventLifecycle.Dispose);
    }

    public void NotifyAngleChanged(int id, float angle)
    {
        Table[id].SetAction(EventLifecycle.Update);
        Table[id].Angle = angle;
    }

    public void NotifyPositionChanged(int id, Vector2 position)
    {
        Table[id].SetAction(EventLifecycle.Update);
        Table[id].Position = position;
    }

    public void NotifySizeChanged(int id, Vector2 size)
    {
        Table[id].SetAction(EventLifecycle.Update);
        Table[id].Size = size;
    }

    public void NotifyTransformed(int id, string newAsset)
    {
        Table[id].SetAction(EventLifecycle.Update);
        Table[id].Asset = newAsset;
    }

    public bool TryTakeMessage(out EventMessage? message)
    {
        message = Table.Serialize();
        Table = new EventTable();
        return message.Disposed?.Count > 0 ||
            message.AngleEvents?.Count > 0 ||
            message.SizeEvents?.Count > 0 ||
            message.Created?.Count > 0 ||
            message.Transformations?.Count > 0 ||
            message.PositionEvents?.Count > 0;
    }
}
