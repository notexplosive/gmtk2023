using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class PowerUp : Entity
{
    public PowerUpType Type { get; }

    public PowerUp(PowerUpType type)
    {
        Type = type;
        Size = new Vector2(10, 10);
    }

    public override void Draw(Painter painter)
    {
        var sheet = Global.MainSheet;
        switch (Type)
        {
            case PowerUpType.HomingShot:
            case PowerUpType.TripleShot:
            case PowerUpType.PiercingShot:
                sheet.DrawFrameAtPosition(painter, 5 + (int) Type, Position, Scale2D.One,
                    new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
                break;
            case PowerUpType.Bomb:
                sheet.DrawFrameAtPosition(painter, 15, Position, Scale2D.One,
                    new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
                break;
            case PowerUpType.Health:
                sheet.DrawFrameAtPosition(painter, 14, Position, Scale2D.One,
                    new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
                break;
        }
    }

    public override void Update(float dt)
    {
        DestroyIfOutOfBounds();
        Position -= new Vector2(0, dt * 30);
    }
}
