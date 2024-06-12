using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Models.Gameplay;
using System.Numerics;

namespace SnakeGame.Systems.Movement;

internal class TrailMovementManager(Dictionary<ClientIdentifier, SnakeCharacter> Characters) : IUpdateService
{
    private const float HeadOffset = 4f;
    public void Update(IGameContext context)
    {
        foreach (var character in Characters.Values)
        {
            character.Transform.Angle = character.Transform.Angle.RotateTowards(
                character.MovementDirection,
                MathF.PI * 2,
                character.RotationSpeed,
                context.DeltaTime);

            var direction = MathEx.AngleToVector(character.Transform.Angle);
            var distance = character.Speed * direction * context.DeltaTime;

            character.Transform.Position += distance;
            character.Head.Transform.Position = character.Transform.Position + direction * HeadOffset;
            character.Head.Transform.Angle = character.Transform.Angle;

            var transit = new TrailNode()
            {
                DistanceTraveled = distance.Length(),
                Position = character.Transform.Position,
                Rotation = character.Transform.Angle
            };

            foreach (var part in character.Body)
            {
                if (transit is not null)
                {
                    part.Transform.Position = transit.First().Position;
                    part.Transform.Angle = transit.First().Rotation;

                    part.Path.ExtendFront(transit);
                    transit = null;
                }
                var extra = Math.Min(part.Path.TotalLength - character.BodyIndentation, character.ShrinkSpeed * context.DeltaTime);
                if (extra > 0)
                {
                    transit = part.Path.Cut(extra);
                }
            }
            
            if (character.Body.Count > 0)
            {
                character.Body[character.Body.Count - 1].Path.Clear();
            }
        }
    }
}
