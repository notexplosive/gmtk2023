using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class EnemyShip : Ship
{
    private readonly int _frame;

    public EnemyShip(int frame) : base(Team.Enemy)
    {
        _frame = frame;
    }
    
    public override void Draw(Painter painter)
    {
        Global.ShipsSheet.DrawFrameAtPosition(painter, _frame, Position, Scale2D.One,
            new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
    }

    public override void Update(float dt)
    {
        Position += new Vector2(0, - 120 * dt);
        DestroyIfOutOfBounds();
    }
}
