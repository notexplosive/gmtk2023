﻿using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class Ship : Entity
{
    public Team Team { get; }
    public int Health { get; private set; }
    public virtual RectangleF TakeDamageBox => BoundingBox.Inflated(-10, -10);

    public Ship(Team team, int health)
    {
        Size = new Vector2(32);
        Team = team;
        Health = health;
    }
    
    public void Shoot(BulletStats bulletStats)
    {
        World.Entities.DeferredActions.Add(() =>
        {
            var bullet = World.Entities.AddImmediate(new Bullet(Team, bulletStats));
            bullet.Position = Position;
        });
    }

    public void GetHitBy(Ship otherShip)
    {
        if (HasInvulnerabilityFrames())
        {
            return;
        }

        TakeDamage();
        otherShip.TakeDamage();
    }

    private void TakeDamage()
    {
        Health--;
        TookDamage?.Invoke();
    }

    public event Action TookDamage;

    public abstract bool HasInvulnerabilityFrames();

    public void GetHitBy(Bullet bullet)
    {
        if (HasInvulnerabilityFrames())
        {
            return;
        }
        
        TakeDamage();

        if (Health <= 0)
        {
            Destroy();
        }
    }
}