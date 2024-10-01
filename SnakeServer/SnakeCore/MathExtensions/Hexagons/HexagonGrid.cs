using System.Numerics;

namespace SnakeCore.MathExtensions.Hexagons;

public class HexagonGrid
{
    readonly struct Span
    {
        public required int Start { get; init; }
        public required float Count { get; init; }
    }

    public float InscribedRadius { get; private init; }
    public float CellSide { get; private init; }
    public float SegmentHeight { get; private init; }

    public HexagonGrid(float inscribedRadius)
    {
        InscribedRadius = inscribedRadius;
        CellSide = InscribedRadius * 2 / MathF.Sqrt(3);
        SegmentHeight = (CellSide * 2 - CellSide) / 2;
    }

    public Vector2 Translate(HexagonTile point)
    {
        return new Vector2(point.Q * InscribedRadius + point.R * InscribedRadius * 2, point.Q * CellSide * 1.5f);
    }

    private static float RoundConditional(float number, bool down)
    {
        return down ? MathF.Floor(number) : MathF.Ceiling(number);
    }

    private static float GetPole(float y, float radius, bool down, float segmentLength)
    {
        var factor = down ? 1 : -1;
        var top = y - radius * factor;
        var tileY = RoundConditional(top / segmentLength, down);
        var topVertexY = RoundConditional((tileY - factor) / 3, down) + factor;
        var topCornerTile = topVertexY * 3 + 2 * factor;
        return topCornerTile;
    }

    public IEnumerable<HexagonTile> Inside(Vector2 position, float radius)
    {
        var topCornerTile = GetPole(position.Y, radius, true, SegmentHeight);
        var bottomCornerTile = GetPole(position.Y, radius, false, SegmentHeight);
        var topBoundTile = MathF.Ceiling(position.Y / SegmentHeight);
        for (var i = topCornerTile + 1; i <= bottomCornerTile; i += 3)
        {
            var isTopHalf = i > topBoundTile;
            var factor = isTopHalf ? -1 : 1;
            var y = i - factor;
            var isBlue = Math.Abs(y % 2) != 0;
            var mapY = Convert.ToInt32(RoundConditional((float)(y - factor) / 3, !isTopHalf)) + factor;

            var side = GetHorizontalSpan(position, y, radius, mapY, isBlue);
            var pointy = GetHorizontalSpan(position, y - factor, radius, mapY, !isBlue);


            for (var offsetX = 0; offsetX < side.Count && offsetX <= pointy.Count; offsetX++)
            {
                yield return new HexagonTile() { Q = mapY, R = Math.Max(side.Start, pointy.Start) + offsetX };
            }
        }
    }

    private Span GetHorizontalSpan(Vector2 position, float i, float radius, float mapY, bool isBlue)
    {
        var sideY = i * SegmentHeight - position.Y;
        var sideX = -MathF.Sqrt(MathF.Pow(radius, 2) - MathF.Pow(sideY, 2)) + position.X;
        var sideLeftCorner = MathF.Ceiling((sideX - (isBlue ? InscribedRadius : 0)) / (InscribedRadius * 2)) * InscribedRadius * 2 + (isBlue ? InscribedRadius : 0);
        var sideSlice = MathF.Floor(((position.X - sideX) * 2 - sideLeftCorner + sideX) / (InscribedRadius * 2));

        var sideMapX = GetSegment(sideLeftCorner, 1 / (InscribedRadius * 2), InscribedRadius - mapY * InscribedRadius);

        return new Span()
        {
            Start = sideMapX,
            Count = sideSlice
        };
    }

    public HexagonTile GetTile(Vector2 position)
    {
        var virtualX = position.X;
        var virtualY = position.Y;

        var tileY = GetSegment(virtualY, 1 / SegmentHeight, 0);
        var tileX = GetSegment(virtualX, 1 / InscribedRadius, 0);

        var isOrange = (tileY - 1) % 3 == 0;

        var segmentY = Convert.ToInt32(MathF.Floor((float)(tileY - 1) / 3)) + 1;

        if (isOrange && CalculateOnPointy(virtualX, virtualY, tileX, tileY))
        {
            segmentY -= 1;
        }

        var segmentX = GetSegment(virtualX, 1 / (InscribedRadius * 2), InscribedRadius - segmentY * InscribedRadius);

        return new HexagonTile()
        {
            Q = segmentY,
            R = segmentX
        };
    }
    
    private bool CalculateOnPointy(float x, float y, float tileX, float tileY)
    {
        var x0 = tileX * InscribedRadius + InscribedRadius / 2;
        var y0 = tileY * SegmentHeight + SegmentHeight / 2;
        var x1 = (x - x0) / (InscribedRadius / 2);
        var y1 = (y - y0) / (SegmentHeight / 2);

        var isBlue = Math.Abs((tileY - 1) % 6) <= 2;

        if (isBlue != (tileX % 2 == 0))
        {
            return x1 > y1;
        }
        else
        {
            return x1 < -y1;
        }
    }

    private static int GetSegment(float x, float factor, float offset)
    {
        return Convert.ToInt32(MathF.Floor((x + offset) * factor));
    }
}
