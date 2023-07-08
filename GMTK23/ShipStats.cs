using Microsoft.Xna.Framework;

namespace GMTK23;

public record ShipStats(int Frame, int Health, float BulletCooldown, Vector2 DealDamageAreaSize,
    BulletStats BulletStats, float Speed, TailStats? Tail = null);

public record TailStats(int Frame, int NumberOfSegments, float BulletCooldown, BulletStats BulletStats);
