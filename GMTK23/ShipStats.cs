using Microsoft.Xna.Framework;

namespace GMTK23;

public class ShipStats
{
    public readonly int Health;

    public ShipStats(int health, Vector2 dealDamageAreaSize)
    {
        Health = health;
        DealDamageAreaSize = dealDamageAreaSize;
    }

    public Vector2 DealDamageAreaSize { get; }
}
