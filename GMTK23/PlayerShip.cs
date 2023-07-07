using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;

namespace GMTK23;

public class PlayerShip : Entity
{
    public override void Draw(Painter painter)
    {
        Global.ShipsSheet.DrawFrameAtPosition(painter, 0, Position, Scale2D.One,
            new DrawSettings {Flip = new XyBool(false, true), Origin = DrawOrigin.Center, Depth = RenderDepth});
    }
}
