using System;
using System.Diagnostics.Contracts;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class HeatmapCell
{
    public float AvoidScore;
    public float DesireScore;

    public HeatmapCell(RectangleF rectangle)
    {
        Rectangle = rectangle;
    }

    public RectangleF Rectangle { get; }
    public Vector2 Position => Rectangle.Center;

    public void Update(float dt)
    {

        DesireScore = MathF.Min(DesireScore.MovedTowardsZero(dt * Heatmap.CoolingIncrement), 1f);
        AvoidScore = MathF.Min(AvoidScore.MovedTowardsZero(dt * Heatmap.CoolingIncrement), 1f);
    }
}

public static class FloatExtensions
{
    [Pure]
    public static float MovedTowardsZero(this float self, float amount)
    {
        if (Math.Abs(self) < amount)
        {
            return 0;
        }

        if (self < 0)
        {
            return self + amount;
        }

        if (self > 0)
        {
            return self - amount;
        }

        return self;
    }
}
