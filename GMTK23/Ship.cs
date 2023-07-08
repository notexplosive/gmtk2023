using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class Ship : Entity
{
    public Team Team { get; }
    public int Health { get; private set; }

    public Ship(Team team, int health)
    {
        Size = new Vector2(32);
        Team = team;
        Health = health;
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
        Health--;

        if (Health < 0)
        {
            Destroy();
        }
    }
}
