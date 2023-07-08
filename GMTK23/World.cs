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
        var shipsThatAreInBounds = Entities.OfType<Ship>().Where(ship => Bounds.Contains(ship.BoundingBox)).ToList();
        var friendlyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Player).ToList();
        var enemyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Enemy).ToList();
        var enemyShipsTypeSafe = enemyShips.OfType<EnemyShip>().ToList();

        foreach (var bullet in bullets)
        {
            var affectedShips = bullet.Team == Team.Player ? enemyShips : friendlyShips;

            foreach (var ship in affectedShips)
            {
                if (bullet.BoundingBox.Overlaps(ship.BoundingBox))
                {
                    bullet.Destroy();
                    ship.GetHitBy(bullet);
                    break;
                }
            }
        }

        foreach (var enemyShip in enemyShipsTypeSafe)
        {
            foreach (var friendlyShip in friendlyShips)
            {
                if (enemyShip.DealDamageBox.Overlaps(friendlyShip.BoundingBox))
                {
                    friendlyShip.GetHitBy(enemyShip);
                }
            }
        }
    }

    public EntityCollection Entities { get; }
    public RectangleF Bounds { get; }
}
