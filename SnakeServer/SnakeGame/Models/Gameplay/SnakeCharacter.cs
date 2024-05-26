using SnakeGame.Common;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Input.Internal;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace SnakeGame.Models.Gameplay;

internal class SnakeCharacter : SquareBody
{
    public TeamColor Team { get; set; }
    public float MovementDirection { get; set; } = 0f;
    public float Speed { get; set; } = 40f;
    public bool OnBase { get; set; } = false;
    public required List<SnakeBodypart> Body { get; init; }
    public required SquareBody Head { get; init; }
    public byte MaxTier => Body.Select(it => it.Tier).Max();

    public void JoinPart(byte tier, FrameFactory factory)
    {
        if (InvokeReaction(tier))
        {
            return;
        }
        var frontElement = Body.Where(it => it.Tier >= tier).LastOrDefault();
        if (frontElement is null)
        {
            Body.Insert(0, new SnakeBodypart()
            {
                Transform = factory.Create($"body{tier}", Transform.ReadOnly),
                Tier = tier
            });
            return;
        }
        var tail = frontElement.Trail.LastOrDefault(new SnakeTrailSegment()
        {
            DistanceTraveled = 0,
            Position = frontElement.Transform.Position,
            Rotation = frontElement.Transform.Angle
        });
        Body.Insert(Body.IndexOf(frontElement) + 1, new SnakeBodypart() 
        { 
            Transform = factory.Create($"body{tier}", Transform.ReadOnly),
            Tier = tier
        });
    }

    private bool InvokeReaction(byte tier)
    {
        if (tier > 30)
        {
            return false;
        }

        foreach (var part in Body.ToArray())
        {
            if (part.Tier == tier)
            {
                if (InvokeReaction((byte)(tier + 1)))
                {
                    Body.Remove(part);
                    part.Transform.Dispose();
                }
                else
                {
                    part.Tier += 1;
                    part.Transform.ChangeAsset($"body{part.Tier}");
                }
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        if (Body.Count == 0)
        {
            return "empty";
        }
        var builder = new StringBuilder();
        foreach (var segment in Body)
        {
            builder.Append(segment.Value);
            builder.Append(">");
        }
        return builder.ToString();
    }
}
