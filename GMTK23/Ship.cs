using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class Ship : TeamedEntity
{
    public Ship(Team team, int health) : base(team)
    {
        Size = new Vector2(32);
        Health = health;
    }

    public int Health { get; private set; }

    public void Shoot(BulletStats bulletStats)
    {
        World.Entities.DeferredActions.Add(() =>
        {
            var bullet = World.Entities.AddImmediate(new Bullet(Team, bulletStats));
            bullet.Position = Position;
        });
    }

    protected override void TakeDamageInternal()
    {
        if (HasInvulnerabilityFrames())
        {
            return;
        }

        Health--;

        if (Health <= 0)
        {
            Destroy();
        }
    }

    public abstract bool HasInvulnerabilityFrames();
}
