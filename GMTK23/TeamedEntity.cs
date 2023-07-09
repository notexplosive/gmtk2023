using System;
using System.Collections.Generic;
using System.Linq;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class TeamedEntity : Entity
{
    public IEnumerable<TeamedEntity> Enemies => OtherEntities.OfType<TeamedEntity>().Where(entity => entity.Team != Team);

    private HashSet<Bullet> _hasBeenHitBy = new();
    public event Action? TookDamage;
    
    public virtual RectangleF TakeDamageBox => BoundingBox.Inflated(-10, -10);
    
    public TeamedEntity(Team team)
    {
        Size = new Vector2(32);
        Team = team;
    }

    public Team Team { get; }

    public void TakeDamage()
    {
        if (TakeDamageInternal())
        {
            TookDamage?.Invoke();
        }
    }

    protected abstract bool TakeDamageInternal();


    public void Shoot(BulletStats bulletStats)
    {
        Shoot(bulletStats, Vector2.Zero);
    }
    
    public void Shoot(BulletStats bulletStats, Vector2 offset)
    {
        World.Entities.DeferredActions.Add(() =>
        {
            var bullet = World.Entities.AddImmediate(new Bullet(Team, bulletStats));
            bullet.Position = Position + offset;
        });

        if (bulletStats.Sound != null)
        {
            Global.PlaySound(bulletStats.Sound);
        }
    }

    public void TakeDamageFrom(Bullet bullet)
    {
        if (!_hasBeenHitBy.Contains(bullet))
        {
            TakeDamage();
        }

        if (bullet.Stats.PowerUpType == PowerUpType.PiercingShot)
        {
            _hasBeenHitBy.Add(bullet);
        }
    }
}
