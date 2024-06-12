using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.Internal;
using System.Numerics;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Systems.Movement;

namespace SnakeGame.Services.Gameplay;

internal class SnakeMovementManager(Dictionary<ClientIdentifier, SnakeCharacter> Players) : IUpdateService
{
    private const float HeadOffset = 4f;
    private const float ShrinkPerFrame = 4f;
    public void Update(IGameContext context)
    {
        foreach (var player in Players.Values)
        {
            player.Transform.Angle = player.Transform.Angle.RotateTowards(
                player.MovementDirection, 
                MathF.PI*2, 
                player.RotationSpeed, 
                context.DeltaTime);

            var direction = MathEx.AngleToVector(player.Transform.Angle);
            var distance = player.Speed * direction * context.DeltaTime;

            player.Transform.Position += distance;
            player.Head.Transform.Position = player.Transform.Position + direction * HeadOffset;
            player.Head.Transform.Angle = player.Transform.Angle;

            var transitSegment = new TrailSegment()
            {
                DistanceTraveled = distance.Length(),
                Position = player.Transform.Position,
                Rotation = player.Transform.Angle
            };
            var transitTrail = new List<TrailSegment>() { transitSegment };
            foreach (var part in player.Body)
            {
                if (transitTrail.Count > 0)
                {
                    transitSegment = transitTrail.Last();

                    part.Transform.Position = transitSegment.Position;
                    part.Transform.Angle = transitSegment.Rotation;

                    foreach (var segment in transitTrail)
                    {
                        part.DistanceCounter += segment.DistanceTraveled;
                        part.Trail.Enqueue(segment);
                    }
                }
                var extra = Math.Min(part.Trail.Sum(it => it.DistanceTraveled) - player.BodyIndentation, ShrinkPerFrame);
                if (extra > 0)
                {
                    var rest = Cut(part.Trail, extra).ToArray();
                    part.DistanceCounter -= extra;

                    transitTrail.Clear();
                    transitTrail.AddRange(rest);
                }
            }
        }
    }

    private IEnumerable<TrailSegment> Splice(IEnumerable<TrailSegment> trail)
    {
        var opening = trail.FirstOrDefault();
        if (opening is null)
        {
            yield break;
        }

        var remaining = trail.Skip(1).SkipWhile(it =>
        {
            if (it.Rotation == opening.Rotation)
            {
                opening.Position = it.Position;
                opening.DistanceTraveled += it.DistanceTraveled;
                return true;
            }
            return false;
        });
        yield return opening;
        foreach (var segment in Splice(remaining))
        {
            yield return segment;
        }
    }

    private IEnumerable<TrailSegment> Cut(Queue<TrailSegment> trail, float distance)
    {
        var remainingDistance = distance;
        while (remainingDistance > 0 && trail.TryPeek(out var tail))
        {
            if (tail.TryCut(remainingDistance, out var rest))
            {
                remainingDistance = 0;
                yield return rest;
            }
            else
            {
                remainingDistance -= tail.DistanceTraveled;
                yield return trail.Dequeue();
            }
        }
    }
}
