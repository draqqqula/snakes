using Microsoft.Extensions.Configuration;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using SnakeCore.MathExtensions.Hexagons;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Systems.Digging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Digging;

internal class Terrain(IGameConfiguration Configuration, IMapProvider MapProvider) : IStartUpService
{
    private HexagonGrid _grid;
    private HexagonBitMap _map;

    public void Start(IGameContext context)
    {
        _grid = new HexagonGrid(Configuration.Get<float>("MapTileSize"));
        _map = MapProvider.GetMap();
    }

    public IEnumerable<int> Dig(Circle explorationCircle, Circle diggingCircle)
    {
        var terrainFound = _grid
            .Inside(explorationCircle.Position, explorationCircle.Radius)
            .Select(coordinates => _map[coordinates.Q, coordinates.R])
            .Any(IsTrue);

        if (!terrainFound)
        {
            yield break;
        }

        var diggedTiles = _grid
            .Inside(diggingCircle.Position, diggingCircle.Radius);

        foreach (var tile in diggedTiles)
        {
            var flag = IsTrue(_map[tile]);
            if (flag && _map.TryGetIndex(tile.Q, tile.R, out var index))
            {
                _map[tile] = false;
                yield return index;
            }
        }
    }

    private bool IsTrue(bool? nullable)
    {
        return nullable.HasValue && nullable.Value;
    }
}