using System;
using System.Collections.Generic;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class TailSegment : TeamedEntity
{
    private readonly int _frame;
    private readonly EnemyShip _master;
    private readonly TailStats _tailStats;
    private readonly Queue<Vector2> _nextPositions;
    private readonly Entity _nextSegment;
    private float _bulletCooldownTimer;
    private bool _isReady;

    public override RectangleF TakeDamageBox => BoundingBox;

    public TailSegment(Entity nextSegment, EnemyShip master, TailStats tailStats) : base(master.Team)
    {
        _nextSegment = nextSegment;
        _master = master;
        _tailStats = tailStats;

        _frame = tailStats.Frame;
        master.Destroyed += () =>
        {
            World.Entities.AddImmediate(new SpriteVfx(Client.Assets.GetAsset<GridBasedSpriteSheet>("Explosion"), Position,
                0.5f));
            World.Entities.RemoveImmediate(this);
        };

        Position = nextSegment.Position;
        _nextPositions = new Queue<Vector2>();

        _bulletCooldownTimer = Client.Random.Clean.NextFloat() * tailStats.BulletCooldown;
    }

    public override void Draw(Painter painter)
    {
        if (!_isReady)
        {
            return;
        }
        var nextPosition = _nextPositions.Peek();

        var sheet = Global.MainSheet;

        if (_master.DamageFlashTimer > 0)
        {
            sheet = Global.MainSheetWithFlash;
        }

        var angle = (nextPosition - Position).GetAngleFromUnitX() - MathF.PI / 2;

        sheet.DrawFrameAtPosition(painter, _frame, Position, Scale2D.One,
            new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth, Angle = angle});
    }

    public override void Update(float dt)
    {
        _nextPositions.Enqueue(_nextSegment.Position);
        
        if (!_isReady)
        {
            Position = _nextPositions.Peek();
            _isReady = _nextPositions.Count > _tailStats.DelayFrames;
        }

        if (!_isReady)
        {
            return;
        }
        
        Position = _nextPositions.Dequeue();

        _bulletCooldownTimer -= dt;
        if (_bulletCooldownTimer < 0)
        {
            _bulletCooldownTimer = _tailStats.BulletCooldown;
            Shoot(_tailStats.BulletStats);
        }
    }

    protected override bool TakeDamageInternal()
    {
        _master.TakeDamage();
        return true;
    }
}
