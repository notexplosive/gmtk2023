using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class EnemyShip : Ship
{
    private readonly SequenceTween _currentTween;
    private readonly int _frame;
    private readonly ShipStats _shipStats;
    private readonly float _speed;
    private float _bulletCooldownTimer;

    public EnemyShip(ShipStats shipStats, ShipChoreoid shipChoreoid) : base(Team.Enemy, shipStats.Health)
    {
        _currentTween = shipChoreoid.GenerateTween(
            this,
            new TweenableFloat(
                () => Position.X,
                val => Position = new Vector2(val, Position.Y)),
            new TweenableFloat(
                () => Position.Y,
                val => Position = new Vector2(Position.X, val))
        );
        _speed = shipStats.Speed;
        _frame = shipStats.Frame;
        _shipStats = shipStats;
        _bulletCooldownTimer = shipStats.BulletCooldown * Client.Random.Clean.NextFloat();

        OnDestroy += () =>
        {
            World.Entities.AddImmediate(new Vfx(Client.Assets.GetAsset<GridBasedSpriteSheet>("Explosion"), Position, 0.5f));
        };
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
            _bulletCooldownTimer -= dt;
            
            if (_bulletCooldownTimer < 0)
            {
                _bulletCooldownTimer = _shipStats.BulletCooldown;
                ShootPreferredBullet();
            }
        }
    }

    public void ShootPreferredBullet()
    {
        Shoot(_shipStats.BulletStats);
    }

    public override bool HasInvulnerabilityFrames()
    {
        return false;
    }
}
