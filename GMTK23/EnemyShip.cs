using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class EnemyShip : Ship
{
    private readonly SequenceTween _currentTween;
    private readonly int _frame;
    private readonly ShipStats _shipStats;
    private float _bulletCooldownTimer;
    private readonly float _speed;

    public EnemyShip(ShipStats shipStats, ShipChoreoid shipChoreoid) : base(Team.Enemy, shipStats.Health)
    {
        _currentTween = shipChoreoid.GenerateTween(new TweenableVector2(() => Position, val => Position = val));
        _speed = shipStats.Speed;
        _frame = shipStats.Frame;
        _shipStats = shipStats;
        _bulletCooldownTimer = shipStats.BulletCooldown * Client.Random.Clean.NextFloat();
    }

    public override RectangleF TakeDamageBox => BoundingBox;

    public RectangleF DealDamageBox =>
        RectangleF.InflateFrom(Position, _shipStats.DealDamageAreaSize.X, _shipStats.DealDamageAreaSize.Y);

    public override void Draw(Painter painter)
    {
        if (Client.Debug.IsActive)
        {
            painter.DrawRectangle(DealDamageBox, new DrawSettings {Color = Color.Red});
        }

        Global.MainSheet.DrawFrameAtPosition(painter, _frame, Position, Scale2D.One,
            new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
    }

    public override void Update(float dt)
    {
        if (_currentTween != null && !_currentTween.IsDone())
        {
            // tween based movement
            _currentTween.Update(dt);

            if (_currentTween.IsDone())
            {
                _currentTween.Clear();
            }
        }
        else
        {
            // default move forward
            Position += new Vector2(0, -60 * dt * _speed);
            DestroyIfOutOfBounds();
        }

        _bulletCooldownTimer -= dt;

        if (_bulletCooldownTimer < 0)
        {
            _bulletCooldownTimer = _shipStats.BulletCooldown;
            Shoot(_shipStats.BulletStats);
        }
    }

    public override bool HasInvulnerabilityFrames()
    {
        return false;
    }
}
