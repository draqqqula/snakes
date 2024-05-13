using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Resolvers;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Frames;
using System.Numerics;

namespace SnakeGame.Mechanics.ViewPort;

internal class ViewPortManager(
    Dictionary<ClientIdentifier, ViewPort> ViewPorts, 
    FrameStorage Storage,
    ICollisionResolver<AxisAlignedBoundingBox, AxisAlignedBoundingBox> Collision 
    ) : 
    ISessionService, IUpdateService
{
    public const float ViewPortSize = 40f;
    public Dictionary<ClientIdentifier, List<int>> Intersections { get; } = [];
    public void OnJoin(IGameContext context, ClientIdentifier id)
    {
        Intersections.Add(id, []);
        ViewPorts.Add(id, new ViewPort() 
        { 
            Transform = new TransformObject() 
            { 
                Angle = 0f,
                Position = Vector2.Zero,
                Size = Vector2.One * ViewPortSize,
            } 
        });
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        Intersections.Remove(id);
        ViewPorts.Remove(id);
    }

    public void Update(IGameContext context)
    {
        foreach (var view in ViewPorts)
        {
            Intersections[view.Key].Clear();
            foreach (var frame in Storage.GetAll())
            {
                if (Collision.IsColliding(view.Value.GetBody().First(), new AxisAlignedBoundingBox()
                {
                    Min = frame.Value.Position - frame.Value.Size * 0.5f, 
                    Max = frame.Value.Position + frame.Value.Size * 0.5f
                }))
                {
                    Intersections[view.Key].Add(frame.Key);
                }
            }
        }
    }
}
