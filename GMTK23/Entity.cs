using ExplogineCore.Data;
using ExplogineMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class Entity
{
    public Vector2 Position { get; set; }
    public abstract void Draw(Painter painter);
    public Depth RenderDepth { get; set; }

    public Entity()
    {
        RenderDepth = Depth.Middle;
    }
}
