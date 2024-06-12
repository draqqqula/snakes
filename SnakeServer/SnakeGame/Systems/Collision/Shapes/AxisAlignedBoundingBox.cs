﻿using System.Numerics;

namespace SnakeGame.Mechanics.Collision.Shapes;

internal readonly record struct AxisAlignedBoundingBox : IFlatShape
{
    public Vector2 Min { get; init; }
    public Vector2 Max { get; init; }

    public Polygon AsPolygon()
    {
        return Polygon.FromVertexes(Min, new Vector2(Min.Y, Max.X), Max, new Vector2(Min.X, Max.Y));
    }

    public AxisAlignedBoundingBox GetBounds()
    {
        return this;
    }
}
