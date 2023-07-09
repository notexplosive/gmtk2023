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
        switch (Type)
        {
            case PowerUpType.HomingShot:
            case PowerUpType.TripleShot:
            case PowerUpType.PiercingShot:
                var sheet = Global.MainSheet;
                sheet.DrawFrameAtPosition(painter, 5 + (int) Type, Position, Scale2D.One,
                    new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
                break;
            case PowerUpType.Bomb:
            // todo
            case PowerUpType.Health:
                // todo
                break;
        }
    }

    public override void Update(float dt)
    {
        DestroyIfOutOfBounds();
        Position -= new Vector2(0, dt * 30);
    }
}
