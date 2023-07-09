using System.Collections.Generic;
using System.Linq;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Bullet : Entity
{
    public BulletStats Stats { get; }
    private readonly float _speed;
    private Entity? _homingTarget;
    
    public IEnumerable<TeamedEntity> Enemies => OtherEntities.OfType<TeamedEntity>().Where(entity => entity.Team != Team);

    public Bullet(Team team, BulletStats bulletStats)
    {
        Stats = bulletStats;
        Team = team;
        _speed = bulletStats.Speed;
        Size = new Vector2(5);
    }

    public Team Team { get; }
    public RectangleF DealDamageBox => BoundingBox;

    public override void Draw(Painter painter)
    {
        var flip = new XyBool(false, true);

        if (Team == Team.Player)
        {
            flip = XyBool.False;
        }
        
        Global.MainSheet.DrawFrameAtPosition(painter, Stats.Frame, Position, Scale2D.One,
            new DrawSettings {Flip = flip, Origin = DrawOrigin.Center, Depth = RenderDepth});
        
        /*
        painter.DrawRectangle(DealDamageBox, new DrawSettings {Depth = RenderDepth});
        */
    }

    public override void Update(float dt)
    {
        var velocity = dt * new Vector2(0, _speed * 60f);
        
        if (Stats.PowerUpType == PowerUpType.HomingShot && (_homingTarget == null || _homingTarget.IsDead))
        {
            var possibleTargets = Enemies.OfType<Ship>().ToList();
            if (possibleTargets.Count > 0)
            {
                _homingTarget = Client.Random.Clean.GetRandomElement(possibleTargets);
            }
            else
            {
                _homingTarget = null;
            }
        }
        
        if (_homingTarget != null)
        {
            velocity = (_homingTarget.Position - Position).Normalized() * velocity.Length();
        }


        if (Team == Team.Enemy)
        {
            velocity.Y = -velocity.Y;
        }

        Position += velocity;
        DestroyIfOutOfBounds();
    }

    public void OnHitTarget()
    {
        if (Stats.PowerUpType == PowerUpType.PiercingShot)
        {
            return;
        }
        Destroy();
    }
}

public enum Team
{
    Enemy,
    Player
}
