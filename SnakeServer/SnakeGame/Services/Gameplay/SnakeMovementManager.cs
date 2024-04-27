using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.Internal;
using System.Numerics;

namespace SnakeGame.Services.Gameplay;

internal class SnakeMovementManager(Dictionary<ClientIdentifier, SnakeCharacter> Players) : IUpdateService
{
    public const float MaxTrailLength = 4f;
    public const float RotationSpeed = MathF.PI * 2f;
    public void Update(IGameContext context)
    {
        foreach (var player in Players.Values)
        {
            player.Rotation = player.Rotation.RotateTowards(player.MovementDirection, MathF.PI*2, RotationSpeed, context.DeltaTime);

            var direction = new Vector2(MathF.Cos(player.Rotation), MathF.Sin(player.Rotation));
            var distance = player.Speed * direction * context.DeltaTime;

            var transitSegment = new SnakeTrailSegment()
            {
                DistanceTraveled = distance.Length(),
                Position = player.Position,
                Rotation = player.Rotation
            };
            foreach (var part in player.Body)
            {
                part.Position = transitSegment.Position;
                part.Rotation = transitSegment.Rotation;
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
            player.Position += distance;
        }
    }
}
