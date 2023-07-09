using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class PlayerShip : Ship
{
    private readonly PlayerPersonality _personality;
    private readonly float _speed = 5f;
    private float _animTimer;
    private float _gunCooldownTimer;
    private InputState _inputState = new();
    private bool _invisibleToggle;
    private float _invulnerableTimer;
    private bool _shouldAnimate;

    public PlayerShip(PlayerPersonality personality) : base(Team.Player, 5)
    {
        _personality = personality;
        TookDamage += OnTookDamage;

        Destroyed += () =>
        {
            var vfx = World.Entities.AddImmediate(new RectVfx());
            vfx.Position = Position;
            vfx.Tween
                .Add(
                    new MultiplexTween()
                        .AddChannel(
                            new SequenceTween().Add(
                            vfx.TweenableSize.TweenTo(
                            new Vector2(100,100),
                            0.7f,
                            Ease.CubicFastSlow)
                            )
                                .Add(
                                    vfx.TweenableSize.TweenTo(
                                        new Vector2(80,80),
                                        0.3f,
                                        Ease.CubicSlowFast)
                                    )
                            )
                        .AddChannel(vfx.TweenableAngle.TweenTo(MathF.PI * 4, 1f, Ease.Linear))
                        .AddChannel(
                            new SequenceTween()
                                .Add(new WaitSecondsTween(0.5f))
                                .Add(vfx.TweenableOpacity.TweenTo(0f, 0.5f, Ease.Linear))
                        )
                );
            Global.PlaySound("gmtk23_explode1");
            Global.MusicPlayer.Stop();
            World.GameOver();
        };
    }

    public Heatmap Heatmap { get; private set; } = null!;

    public Vector2 TargetPosition { get; set; }

    private void OnTookDamage()
    {
        World.PlayerStatistics.Intensity += 20;
        Global.PlaySound("gmtk23_enemy4");
        Client.Debug.Log($"Took Damage {Health}");
            _invulnerableTimer = 1f;
    }

    public override void Awake()
    {
        Heatmap = new Heatmap(World.Bounds.Size, 10);
    }

    public override void Draw(Painter painter)
    {
        if (Client.Debug.IsActive)
        {
            Heatmap.DebugDraw(painter);
        }

        if (_invulnerableTimer > 0)
        {
            _invisibleToggle = !_invisibleToggle;

            if (_invisibleToggle)
            {
                return;
            }
        }

        var frame = 2;
        if (_shouldAnimate)
        {
            frame = (int) _animTimer;
        }

        Global.PlayerSheet.DrawFrameAtPosition(painter, frame, Position, Scale2D.One,
            new DrawSettings {Flip = new XyBool(false, true), Origin = DrawOrigin.Center, Depth = RenderDepth});
    }

    public override void Update(float dt)
    {
        if (!World.IsStarted)
        {
            return;
        }
        
        Heatmap.Update(dt);

        _invulnerableTimer -= dt;

        if (_shouldAnimate)
        {
            _animTimer += dt * 20;

            if (_animTimer >= 5)
            {
                _shouldAnimate = false;
                _animTimer = 0;
            }
        }

        Heatmap.Zonify(_personality.PreferredZone(World.Bounds.Size), dt);

        foreach (var enemy in Enemies)
        {
            if (enemy is EnemyShip enemyShip)
            {
                Heatmap.Zonify(enemyShip.DealDamageBox.Inflated(0, 40).Moved(new Vector2(0, -30)), -dt);
            }

            var desiredZone = enemy.TakeDamageBox.Inflated(-10, 0);
            Heatmap.Zonify(RectangleF.FromCorners(new Vector2(desiredZone.X, 0), desiredZone.BottomRight),
                dt * Heatmap.CoolingIncrement);
        }

        foreach (var bullet in Bullets)
        {
            if (bullet.Team == Team.Enemy)
            {
                var bulletRect = bullet.DealDamageBox.Inflated(5, 5);
                Heatmap.Zonify(
                    RectangleF.FromCorners(bulletRect.TopLeft - new Vector2(0, 5 * _speed), bulletRect.BottomRight),
                    -dt * 2);
            }
        }

        _gunCooldownTimer -= dt;
        ExecuteInput(dt);

        Position += _inputState.ToVector2() * _speed;
        Position = new RectangleF(Position, Vector2.Zero).ConstrainedTo(World.Bounds).Center;
    }

    private void ExecuteInput(float dt)
    {
        var movementReacted = Client.Random.Clean.NextFloat() < _personality.MovementReactionSkillPercent();
        if (movementReacted)
        {
            var cellsWithinHitBox = Heatmap.GetCellsWithin(BoundingBox).ToList();
            if (cellsWithinHitBox.Any(a => a.AvoidScore > 0))
            {
                World.PlayerStatistics.Intensity += cellsWithinHitBox.Count * dt * 20;
                _inputState = MoveAwayFromBad();
            }
            else
            {
                _inputState = MoveTowardDesired();
            }
        }

        if (Enemies.Any())
        {
            var shootReacted = Client.Random.Clean.NextFloat() < _personality.ShootReactionSkillPercent();
            if (shootReacted && GunIsCooledDown())
            {
                _gunCooldownTimer = 0.1f;
                Shoot(ScriptContent.PlayerBullet);
                _shouldAnimate = true;
            }
        }
    }

    private InputState MoveAwayFromBad()
    {
        var target = Position;
        foreach (var cell in Heatmap.GetCellsWithin(BoundingBox))
        {
            if (cell.AvoidScore > 0)
            {
                target += Position - cell.Position;
            }
        }

        var result = InputStateForTarget(target);
        return result;
    }

    private InputState MoveTowardDesired()
    {
        var candidates = new List<HeatmapCell>();
        foreach (var cell in Heatmap.GetCellsWithin(_personality.ComfortZone(World.Bounds.Size)))
        {
            if (cell.AvoidScore <= 0 && cell.DesireScore > 0)
            {
                candidates.Add(cell);
            }
        }

        // lowest length should be at the front
        candidates.Sort((a, b) =>
            (a.Position - Position).LengthSquared().CompareTo((b.Position - Position).LengthSquared()));

        // cut off half the list, so we only consider things that are nearby
        candidates.RemoveRange(candidates.Count / 2, candidates.Count / 2);

        // highest desire should be at the front of the list
        candidates.Sort((a, b) => b.DesireScore.CompareTo(a.DesireScore));

        if (candidates.Count == 0)
        {
            return new InputState();
        }

        var randomRange = Math.Min(candidates.Count, 5);
        var winnerIndex = Client.Random.Clean.NextInt(0, randomRange);
        var winner = candidates[winnerIndex];

        var attempts = 10;
        var gaveUp = false;
        while (!CanSafelyReach(candidates[winnerIndex].Position, _personality.RiskTolerance()))
        {
            if (attempts < 0 || winnerIndex >= candidates.Count)
            {
                gaveUp = true;
                break;
            }

            winnerIndex++;
            attempts--;
        }

        if (gaveUp)
        {
            return new InputState();
        }

        return InputStateForTarget(winner.Position);
    }

    private InputState InputStateForTarget(Vector2 target)
    {
        var finalTarget =
            target + Client.Random.Clean.NextNormalVector2() * _personality.Clumsiness();
        var difference = finalTarget - Position;

        if (difference.Length() < _personality.HowCloseItWantsToBeToTargetPosition())
        {
            return new InputState();
        }

        return new InputState
        {
            Horizontal = difference.X,
            Vertical = difference.Y
        };
    }

    private bool CanSafelyReach(Vector2 position, float tolerance)
    {
        var avoidance = 0f;
        foreach (var cell in Heatmap.SweepCellsAlong(TakeDamageBox, TakeDamageBox.Moved(position)))
        {
            avoidance += cell.AvoidScore;

            if (avoidance > tolerance)
            {
                return false;
            }
        }

        return true;
    }

    private bool GunIsCooledDown()
    {
        return _gunCooldownTimer < 0;
    }

    public override bool HasInvulnerabilityFrames()
    {
        return _invulnerableTimer > 0;
    }

    public class InputState
    {
        public float Horizontal;
        public float Vertical;

        public Vector2 ToVector2()
        {
            var vec = new Vector2(Horizontal, Vertical);

            if (vec.LengthSquared() > 0)
            {
                // apparently normalizing a 0,0 gives you NaN, who knew!
                return vec.Normalized();
            }

            return vec;
        }
    }
}
