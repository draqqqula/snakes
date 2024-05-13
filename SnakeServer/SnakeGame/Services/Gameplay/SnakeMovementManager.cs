using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.Internal;
using System.Numerics;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Bodies;

namespace SnakeGame.Services.Gameplay;

internal class SnakeMovementManager(Dictionary<ClientIdentifier, SnakeCharacter> Players) : IUpdateService
{
    public const float MaxTrailLength = 4f;
    public const float RotationSpeed = MathF.PI * 2f;
    public void Update(IGameContext context)
    {
        foreach (var player in Players.Values)
        {
            player.Transform.Angle = player.Transform.Angle.RotateTowards(player.MovementDirection, MathF.PI*2, RotationSpeed, context.DeltaTime);

            var direction = new Vector2(MathF.Cos(player.Transform.Angle), MathF.Sin(player.Transform.Angle));
            var distance = player.Speed * direction * context.DeltaTime;

            player.Head.Transform.Position = player.Transform.Position + direction * MaxTrailLength;
            player.Head.Transform.Angle = player.Transform.Angle;

            var transitSegment = new SnakeTrailSegment()
            {
                DistanceTraveled = distance.Length(),
                Position = player.Transform.Position,
                Rotation = player.Transform.Angle
            };
            foreach (var part in player.Body)
            {
                part.Transform.Position = transitSegment.Position;
                part.Transform.Angle = transitSegment.Rotation;
                part.DistanceCounter += transitSegment.DistanceTraveled;

                part.Trail.Enqueue(transitSegment);
                while (part.DistanceCounter > MaxTrailLength)
                {
                    var tail = part.Trail.Dequeue();
                    part.DistanceCounter -= tail.DistanceTraveled;
                }
                if (part.Trail.Count > 0)
                {
                    transitSegment = part.Trail.First();
                }
            }
            player.Transform.Position += distance;
        }
    }
}
