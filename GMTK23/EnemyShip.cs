using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class EnemyShip : Ship
{
    private readonly int _frame;
    private readonly ShipStats _shipStats;
    private float _bulletCooldownTimer;

    public EnemyShip(ShipStats shipStats) : base(Team.Enemy, shipStats.Health)
    {
        _frame = shipStats.Frame;
        _shipStats = shipStats;
        _bulletCooldownTimer = shipStats.BulletCooldown * Client.Random.Clean.NextFloat();
    }

    public RectangleF DealDamageBox =>
        RectangleF.InflateFrom(Position, _shipStats.DealDamageAreaSize.X, _shipStats.DealDamageAreaSize.Y);

    public override void Draw(Painter painter)
    {
        if (Client.Debug.IsActive)
        {
            painter.DrawRectangle(DealDamageBox, new DrawSettings{Color = Color.Red});
        }
        
        Global.ShipsSheet.DrawFrameAtPosition(painter, _frame, Position, Scale2D.One,
            new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
    }

    public override void Update(float dt)
    {
        Position += new Vector2(0, -80 * dt);
        DestroyIfOutOfBounds();
        _bulletCooldownTimer -= dt;

        if (_bulletCooldownTimer < 0)
        {
            _bulletCooldownTimer = _shipStats.BulletCooldown;
            Shoot();
        }
    }

    public override bool HasInvulnerabilityFrames()
    {
        return false;
    }
}
