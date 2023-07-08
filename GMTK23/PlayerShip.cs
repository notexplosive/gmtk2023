using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class PlayerShip : Ship
{
    public Heatmap Heatmap { get; private set; }
    private readonly PlayerPersonality _personality;
    private readonly float _speed = 5f;
    private float _gunCooldownTimer;
    private InputState _inputState = new();

    public PlayerShip(PlayerPersonality personality) : base(Team.Player, 1)
    {
        _personality = personality;
    }

    public override void Awake()
    {
        Heatmap = new Heatmap(World.Bounds.Size, 10);
    }

    public Vector2 TargetPosition { get; set; }

    public override void Draw(Painter painter)
    {
        Global.ShipsSheet.DrawFrameAtPosition(painter, 0, Position, Scale2D.One,
            new DrawSettings {Flip = new XyBool(false, true), Origin = DrawOrigin.Center, Depth = RenderDepth});

        if (Client.Debug.IsActive)
        {
            Heatmap.DebugDraw(painter);
        }
    }

    public override void Update(float dt)
    {
        Heatmap.Update(dt);

        Heatmap.Zonify(_personality.PreferredZone(World.Bounds.Size), dt);
        
        foreach (var enemy in Enemies)
        {
            Heatmap.Zonify(enemy.HitBox.Inflated(10, 40).Moved(new Vector2(0,-30)), -dt);

            var desiredZone = enemy.HitBox.Inflated(-5, 0);
            Heatmap.Zonify(RectangleF.FromCorners(new Vector2(desiredZone.X, 0), desiredZone.BottomRight), dt * Heatmap.CoolingIncrement);
        }

        _gunCooldownTimer -= dt;
        ExecuteInput();

        Position += _inputState.ToVector2() * _speed;
        Position = new RectangleF(Position, Vector2.Zero).ConstrainedTo(World.Bounds).Center;
    }

    private void ExecuteInput()
    {
        var candidates = new List<HeatmapCell>();
        foreach (var cell in Heatmap.GetCellsWithin(_personality.ComfortZone(World.Bounds.Size)))
        {
            if (cell.AvoidScore <= 0 && cell.DesireScore > 0)
            {
                candidates.Add(cell);
            }
        }
        
        // highest desire should be at the front of the list
        candidates.Sort((a, b) => b.DesireScore.CompareTo(a.DesireScore));

        if (candidates.Count == 0)
        {
            return;
        }

        var randomRange = Math.Min(candidates.Count, 5);
        var winnerIndex = Client.Random.Clean.NextInt(0, randomRange);
        var winner = candidates[winnerIndex];

        var attempts = 10;
        bool gaveUp = false;
        while(!CanSafelyReach(candidates[winnerIndex].Position, 0.1f))
        {
            if (attempts < 0 || winnerIndex >= candidates.Count)
            {
                gaveUp = true;
                break;
            }
            
            winnerIndex++;
            attempts--;
        }

        var targetPosition =
            winner.Position + Client.Random.Clean.NextNormalVector2() * _personality.Clumsiness();
        var difference = targetPosition - Position;
        var movementReacted = Client.Random.Clean.NextFloat() < _personality.MovementReactionSkillPercent();
        var shootReacted = Client.Random.Clean.NextFloat() < _personality.ShootReactionSkillPercent();

        if (movementReacted)
        {
            var isCloseEnough = difference.Length() < _personality.HowCloseItWantsToBeToTargetPosition();
            if (isCloseEnough || gaveUp)
            {
                _inputState = new InputState
                {
                    Horizontal = 0,
                    Vertical = 0
                };
            }
            else
            {
                _inputState = new InputState
                {
                    Horizontal = difference.X,
                    Vertical = difference.Y
                };
            }
        }

        if (shootReacted && GunIsCooledDown())
        {
            _gunCooldownTimer = 0.1f;
            Shoot();
        }
    }

    private bool CanSafelyReach(Vector2 position, float tolerance)
    {
        var avoidance = 0f;
        foreach (var cell in Heatmap.GetCellsAlong(Position, position))
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