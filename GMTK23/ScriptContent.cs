using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GMTK23;

public static class ScriptContent
{
    public static ShipStats BasicEnemy => new(1, 2, 1, new Vector2(10));

    public static IEnumerable<Summon> Summons(Game game)
    {
        var worldBounds = game.World.Bounds;
        var summon = new Summon(game);

        summon.SpawnShipAt(BasicEnemy, new Vector2(worldBounds.Width / 3,worldBounds.Bottom + 64));
        summon.SpawnShipAt(BasicEnemy, new Vector2(worldBounds.Width * 2/3f,worldBounds.Bottom + 64));

        yield return summon;
    }
}
