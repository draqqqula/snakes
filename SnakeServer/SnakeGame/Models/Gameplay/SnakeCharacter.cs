using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Models.Input.Internal;
using System.Drawing;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal record SnakeCharacter : IBodyComponent<RotatableSquare>
{
    public const float Size = 4;
    public SnakeCharacter(Vector2 position, float direction, float speed)
    {
        Position = position;
        MovementDirection = direction;
        Rotation = direction;
        Speed = speed;
        Body = [new SnakeBodypart() 
                { 
                    Position = Position, Rotation = Rotation
                }];
    }

    public void JoinPart(byte tier)
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
                Position = Position,
                Rotation = Rotation,
                Tier = tier
            });
            return;
        }
        var tail = frontElement.Trail.Last();
        Body.Insert(Body.IndexOf(frontElement) + 1, new SnakeBodypart() 
        { 
            Position = tail.Position,
            Rotation = tail.Rotation,
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
                }
                else
                {
                    part.Tier += 1;
                }
                return true;
            }
        }
        return false;
    }

    public float MovementDirection { get; set; } = 0f;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public float Speed { get; set; } = 0.05f;
    public List<SnakeBodypart> Body { get; } = [];
    public IEnumerable<RotatableSquare> GetBody()
    {
        yield return new RotatableSquare()
        {
            Position = Position,
            Rotation = Rotation,
            Size = Size,
        };
    }
}
