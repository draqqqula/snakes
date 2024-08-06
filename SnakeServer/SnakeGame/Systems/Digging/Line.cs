using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Digging
{
    internal class Line
    {
        private readonly List<Vector2> _points = [];
        public IEnumerable<Vector2> Points => _points;
    }
}
