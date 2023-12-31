﻿using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class Ship : TeamedEntity
{
    public Ship(Team team, int health) : base(team)
    {
        Size = new Vector2(32);
        Health = health;
        MaxHealth = health;
    }

    public int MaxHealth { get; }

    public int Health { get; private set; }

    protected void Heal()
    {
        Health++;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    protected override bool TakeDamageInternal()
    {
        if (HasInvulnerabilityFrames())
        {
            return false;
        }

        Health--;

        if (Health <= 0)
        {
            Destroy();
        }

        return true;
    }

    public abstract bool HasInvulnerabilityFrames();
}
