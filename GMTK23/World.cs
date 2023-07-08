using System.Linq;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class World : IUpdateHook
{
    public World(Vector2 worldSize)
    {
        Entities = new EntityCollection(this);
        Bounds = new RectangleF(Vector2.Zero, worldSize);
    }

    public void Update(float dt)
    {
        var bullets = Entities.OfType<Bullet>().ToList();
        var ships = Entities.OfType<Ship>().ToList();
        var friendlyShips = ships.Where(ship => ship.Team == Team.Player).ToList();
        var enemyShips = ships.Where(ship => ship.Team == Team.Enemy).ToList();

        foreach (var bullet in bullets)
        {
            var affectedShips = bullet.Team == Team.Player ? enemyShips : friendlyShips;

            foreach (var ship in affectedShips)
            {
                if (RectangleF.Intersect(bullet.HitBox, ship.HitBox).Area > 0)
                {
                    bullet.Destroy();
                    ship.GetHitBy(bullet);
                    break;
                }
            }
        }
    }

    public EntityCollection Entities { get; }
    public RectangleF Bounds { get; }
}
