using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GMTK23;

public static class ScriptContent
{
    public static BulletStats PlayerBullet => new(10);
    
    public static BulletStats BasicEnemyBullet => new(3);
    public static ShipStats BasicEnemy => new(1, 2, 3, new Vector2(10), BasicEnemyBullet, 1f);

    public static IEnumerable<Summon> Summons(Game game)
    {
        var worldBounds = game.World.Bounds;
        var summon = new Summon(game, new SummonStats(1f));

        summon.SpawnShipAt(BasicEnemy, new Vector2(worldBounds.Width / 8,worldBounds.Bottom + 64));
        summon.SpawnShipAt(BasicEnemy, new Vector2(worldBounds.Width * 7/8f,worldBounds.Bottom + 64));
        summon.SpawnShipAt(BasicEnemy, new Vector2(worldBounds.Width / 3,worldBounds.Bottom + 64));
        summon.SpawnShipAt(BasicEnemy, new Vector2(worldBounds.Width * 2/3f,worldBounds.Bottom + 64));

        yield return summon;
    }
}
