using Microsoft.Extensions.Configuration;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using SnakeCore.MathExtensions.Hexagons;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Systems.Digging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Digging;

internal class Terrain(IGameConfiguration Configuration, IMapProvider MapProvider)
{
    private readonly HexagonGrid _grid = new HexagonGrid(Configuration.Get<float>("MapTileSize"));
    private readonly HexagonBitMap _map = MapProvider.GetMap();

    public bool TryDig(Circle circle)
    {
        var tiles = _grid.Inside(circle.Position, circle.Radius);
        var terrainTiles = tiles.Select(coordinates => (coordinates, _map[coordinates.Q, coordinates.R]))
            .Where(it => it.Item2.HasValue && it.Item2.Value);
        if (terrainTiles.Any() )
        {
            foreach (var tile in terrainTiles)
            {
                _map[tile.coordinates] = false;
            }
            return true;
        }
        return false;
    }
}