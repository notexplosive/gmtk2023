using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class Ship : Entity
{
    public Team Team { get; }

    public Ship(Team team)
    {
        Size = new Vector2(32);
        Team = team;
    }
    
    public void Shoot()
    {
        World.Entities.DeferredActions.Add(() =>
        {
            var bullet = World.Entities.AddImmediate(new Bullet(Team,10));
            bullet.Position = Position;
        });
    }

    public void GetHitBy(Bullet bullet)
    {
        Destroy();
    }
}
