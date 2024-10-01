using SnakeCore.MathExtensions.Hexagons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeCore.MathExtensions;

public class QRCoordinateBasedHexagonMap : HexagonBitMap
{
    private readonly int _startQ;
    private readonly int _endQ;
    private readonly int _startR;
    private readonly int _endR;
    public QRCoordinateBasedHexagonMap(BitArray data, int startQ, int startR, int endQ, int endR) : base(data)
    {
        _startQ = startQ;
        _endQ = endQ;
        _startR = startR;
        _endR = endR;
    }

    protected override bool TryGetIndex(int q, int r, out int index)
    {
        var half = Convert.ToInt32(MathF.Floor((float)q / 2));
        var r0 = r + half;
        if (q < _startQ || q >= _endQ || r0 < _startR || r0 >= _endR)
        {
            index = 0;
            return false;
        }
        index = (q - _startQ) * (_endR - _startR) + r0 - _startR;
        return true;
    }
}
