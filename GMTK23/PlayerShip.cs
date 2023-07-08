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
    private PlayerState _state;

    public PlayerShip(PlayerPersonality personality) : base(Team.Player, 1)
    {
        _personality = personality;
        _state = IdleState.Instance;
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

        Heatmap.DebugDraw(painter);
    }

    public override void Update(float dt)
    {
        Heatmap.Update(dt);
        _state = _state.UpdateState(dt, this, World);
        /*
        var desiredPosition = Position;
        foreach (var entity in OtherEntities)
        {
            var distanceTo = (entity.Position - Position).Length();
            var amountInPersonalSpace = distanceTo - _personality.PersonalSpaceRadius();
            if (amountInPersonalSpace > 0)
            {
                
            }
        }*/

        _gunCooldownTimer -= dt;
        ExecuteInput();

        Position += _inputState.ToVector2() * _speed;
        Position = new RectangleF(Position, Vector2.Zero).ConstrainedTo(World.Bounds).Center;
    }

    private void ExecuteInput()
    {
        var targetPosition =
            TargetPosition + Client.Random.Clean.NextNormalVector2() * _personality.Clumsiness();
        var difference = targetPosition - Position;
        var movementReacted = Client.Random.Clean.NextFloat() < _personality.MovementReactionSkillPercent();
        var shootReacted = Client.Random.Clean.NextFloat() < _personality.ShootReactionSkillPercent();

        if (movementReacted)
        {
            if (difference.Length() > _personality.HowCloseItWantsToBeToTargetPosition())
            {
                _inputState = new InputState
                {
                    Horizontal = difference.X,
                    Vertical = difference.Y
                };
            }
            else
            {
                _inputState = new InputState
                {
                    Horizontal = 0,
                    Vertical = 0
                };
            }
        }

        if (shootReacted && GunIsCooledDown())
        {
            _gunCooldownTimer = 0.1f;
            Shoot();
        }
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