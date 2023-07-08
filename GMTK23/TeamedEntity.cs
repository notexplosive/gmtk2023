using System;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class TeamedEntity : Entity
{
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
        World.Entities.DeferredActions.Add(() =>
        {
            var bullet = World.Entities.AddImmediate(new Bullet(Team, bulletStats));
            bullet.Position = Position;
        });

        if (bulletStats.Sound != null)
        {
            Global.PlaySound(bulletStats.Sound);
        }
    }
}
