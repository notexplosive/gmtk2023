using System;
using System.Diagnostics.Contracts;
using ExplogineMonoGame.Data;

namespace GMTK23;

public class HeatmapCell
{
    public float Want;
    public float DontWant;

    public HeatmapCell(RectangleF rectangle)
    {
        Rectangle = rectangle;
    }

    public void Update(float dt)
    {
        Want = Want.MovedTowardsZero(dt);
        DontWant = DontWant.MovedTowardsZero(dt);
    }
    
    public RectangleF Rectangle { get; }
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
