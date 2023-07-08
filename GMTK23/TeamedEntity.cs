using System;
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
        TakeDamageInternal();
        TookDamage?.Invoke();
    }

    protected abstract void TakeDamageInternal();
    
    public void GetHitBy(TeamedEntity otherShip)
    {
        TakeDamageInternal();
        otherShip.TakeDamageInternal();
    }
}
