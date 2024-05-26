using MessageSchemes;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeCore.Extensions;
using SnakeGame.Common;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Output.Internal;
using System.Numerics;
using System.Security.Claims;

namespace SnakeGame.Services.Gameplay.FrameDrivers;

internal class AreaSpawner(
    Dictionary<TeamColor, TeamContext> Teams,
    List<PickupPoints> Pickups,
    ICollisionChecker Collision,
    Dictionary<ClientIdentifier, SnakeCharacter> Players,
    FrameFactory Factory
    ) : IUpdateService
{
    private const float MinRadius = 40f;
    private IEnumerable<TeamArea> Areas => Teams.Values.Select(it => it.Area);

    public void Update(IGameContext context)
    {
    }

    public void CheckPlayers()
    {
        foreach (var player in Players.Values)
        {
            var area = Teams[player.Team].Area;
            if (Vector2.Distance(player.Head.Transform.Position, area.Transform.Position) <= area.Radius)
            {
                player.OnBase = true;
                continue;
            }
            if (player.OnBase)
            {
                DropPoints(player);
            }
            player.OnBase = false;
        }
    }

    public void ApplyAttraction(float deltaTime)
    {
        foreach (var team in Teams)
        {
            var claimed = Pickups
                .Where(it => Vector2.Distance(it.Transform.Position, team.Value.Area.Transform.Position) < team.Value.Area.Radius).ToArray();

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
                        var distance = Vector2.Distance(item.Transform.Position, pickup.Transform.Position);
                        return (item, distance);
                    })
                    .OrderBy(it => it.distance))
                {
                    if (pickup2.distance < 4)
                    {
                        Pickups.Remove(pickup);
                        pickup.Transform.Dispose();
                        pickup2.item.Tier += 1;
                        pickup2.item.Transform.ChangeAsset($"pickup{pickup2.item.Tier}");
                        break;
                    }
                    pickup.Transform.Position +=
                        Vector2.Normalize(pickup2.item.Transform.Position - pickup.Transform.Position) *
                        deltaTime * 5;
                }
            }

            var balance = claimed
                .Sum(it => it.Value);
            team.Value.Area.Radius = MinRadius + MathF.Max(0, MathF.Sqrt(balance));
        }
    }

    private void DropPoints(SnakeCharacter character)
    {
        foreach (var segment in character.Body.Skip(1).ToArray())
        {
            SpawnPickup(segment);
            character.Body.Remove(segment);
            segment.Transform.Dispose();
        }
    }

    private void SpawnPickup(SnakeBodypart segment)
    {
        Pickups.Add(new PickupPoints()
        {
            Transform = Factory.Create($"pickup{segment.Tier}", segment.Transform.ReadOnly),
            Tier = segment.Tier,
        });
    }
}
