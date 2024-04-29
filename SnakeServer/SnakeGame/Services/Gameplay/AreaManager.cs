using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Common;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Output.Internal;
using System.Numerics;
using System.Security.Claims;

namespace SnakeGame.Services.Gameplay;

internal class AreaManager(
    Dictionary<TeamColor, TeamContext> Teams, 
    List<PickupPoints> Pickups,
    ICollisionChecker Collision,
    Dictionary<ClientIdentifier, SnakeCharacter> Players
    ) : IUpdateService, IOutputService<FrameDisplayOutput>
{
    private const float MinRadius = 40f;
    private IEnumerable<TeamArea> Areas => Teams.Values.Select(it => it.Area);
    public IEnumerable<FrameDisplayOutput> Pass()
    {
        foreach (var area in Areas)
        {
            yield return new FrameDisplayOutput()
            {
                Name = "area",
                Position = area.Center,
                Rotation = 0f,
                Scale = area.Radius * 2 * Vector2.One
            };
        }
    }

    public void Update(IGameContext context)
    {
        foreach (var pickup in Pickups)
        {
            pickup.Claim = null;
        }

        foreach (var player in Players.Values)
        {
            var area = Teams[player.Team].Area;
            if (Vector2.Distance(player.Position, area.Center) <= area.Radius)
            {
                player.OnBase = true;
                return;
            }
            if (player.OnBase)
            {
                DropPoints(player);
            }
            player.OnBase = false;
        }

        foreach (var team in Teams)
        {
            var claimed = Pickups
                .Where(it => Vector2.Distance(it.Position, team.Value.Area.Center) < team.Value.Area.Radius).ToArray();

            foreach (var pickup in claimed)
            {
                if (!Pickups.Contains(pickup))
                {
                    return;
                }
                pickup.Claim = team.Key;
                foreach (var pickup2 in claimed
                    .Except([pickup])
                    .Where(it => it.Tier == pickup.Tier)
                    .Select(item =>
                    {
                        var distance = Vector2.Distance(item.Position, pickup.Position);
                        return (item, distance);
                    })
                    .OrderBy(it => it.distance))
                {
                    if (pickup2.distance < 4)
                    {
                        Pickups.Remove(pickup);
                        pickup2.item.Tier += 1;
                        break;
                    }
                    pickup.Position += 
                        Vector2.Normalize(pickup2.item.Position - pickup.Position) * 
                        context.DeltaTime * 5;
                }
            }

            var balance = claimed
                .Sum(it => it.Value);
            team.Value.Area.Radius = MinRadius + MathF.Max(0, MathF.Sqrt(balance));
        }
    }

    private void DropPoints(SnakeCharacter character)
    {
        foreach(var segment in character.Body.Skip(1).ToArray())
        {
            SpawnPickup(segment);
            character.Body.Remove(segment);
        }
    }

    private void SpawnPickup(SnakeBodypart segment)
    {
        Pickups.Add(new PickupPoints()
        {
            Position = segment.Position,
            Rotation = segment.Rotation,
            Tier = segment.Tier,
        });
    }
}
