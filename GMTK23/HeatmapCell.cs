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

    public RectangleF Rectangle { get; }
}
