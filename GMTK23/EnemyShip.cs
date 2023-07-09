﻿using System.Collections.Generic;
using System.Linq;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class EnemyShip : Ship
{
    private readonly SequenceTween? _currentTween;
    private readonly int _frame;
    private readonly ShipStats _shipStats;
    private readonly float _speed;
    private float _bulletCooldownTimer;
    private readonly bool _doesNotShoot;
    public float DamageFlashTimer { get; private set; }

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
        _doesNotShoot = _shipStats.BulletCooldown == 0;

        Destroyed += () =>
        {
            if (World.Bounds.Contains(Position))
            {
                World.Entities.AddImmediate(new SpriteVfx(Client.Assets.GetAsset<GridBasedSpriteSheet>("Explosion"),
                    Position, 0.5f));
                World.ScoreDoober(Position, (int) (_shipStats.Health * _shipStats.BulletStats.Speed) * 100);
                Global.PlaySound("gmtk23_enemy3");
                World.PlayerStatistics.Intensity -= (_shipStats.Health * _shipStats.BulletStats.Speed) / 2f;

                if (World.Entities.Any(ent => ent is EnemyShip))
                {
                    World.PlayerStatistics.Intensity += 5;
                }
            }
        };
        TookDamage += () =>
        {
            DamageFlashTimer = 2f/60;
            if (Health > 0)
            {
                Global.PlaySound("gmtk23_enemy6");
            }
        };
    }

    private void SetupTail(TailStats tailStats)
    {
        var tailSegment = World.Entities.AddImmediate(new TailSegment(this, this, tailStats));

        for (int i = 1; i < tailStats.NumberOfSegments; i++)
        {
            tailSegment = World.Entities.AddImmediate(new TailSegment(tailSegment, this, tailStats));
        }
    }

    public override RectangleF TakeDamageBox => BoundingBox;

    public RectangleF DealDamageBox =>
        RectangleF.InflateFrom(Position, _shipStats.DealDamageAreaSize.X, _shipStats.DealDamageAreaSize.Y);

    public override void Awake()
    {
        if (_shipStats.Tail != null)
        {
            SetupTail(_shipStats.Tail);
        }
    }
    
    public override void Draw(Painter painter)
    {
        if (Client.Debug.IsActive)
        {
            painter.DrawRectangle(DealDamageBox, new DrawSettings {Color = Color.Red});
        }

        var sheet = Global.MainSheet;

        if (DamageFlashTimer > 0)
        {
            sheet = Global.MainSheetWithFlash;
        }

        sheet.DrawFrameAtPosition(painter, _frame, Position, Scale2D.One,
            new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth});
    }

    public override void Update(float dt)
    {
        DamageFlashTimer -= dt;

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
        if (_doesNotShoot)
        {
            return;
        }
        Shoot(_shipStats.BulletStats);
    }

    public override bool HasInvulnerabilityFrames()
    {
        return false;
    }
}
