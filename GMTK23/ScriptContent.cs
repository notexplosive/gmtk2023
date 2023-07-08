﻿using System.Collections.Generic;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public static class ScriptContent
{
    public static BulletStats PlayerBullet => new(10, 34);

    public static BulletStats BasicEnemyBullet => new(3, 34);
    public static ShipStats BasicEnemy => new(1, 2, 3, new Vector2(10), ScriptContent.BasicEnemyBullet, 1f);

    public static IEnumerable<Summon> Summons(Game game)
    {
        var worldBounds = game.World.Bounds;
        var worldBoundsInset = worldBounds.Inflated(-20, -20);
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var summon = new Summon(game, new SummonStats(1f));

        var main = summon.AddChoreoid(ScriptContent.BasicEnemy);

        var rightMiddleSpawn = new Vector2(worldBoundsOutset.Right, worldBounds.Center.Y);
        var leftMiddleSpawn = new Vector2(worldBoundsOutset.Left, worldBounds.Center.Y);

        main.AddSpawnEvent(leftMiddleSpawn)
            .AddMoveTo(new Vector2(worldBounds.Width / 8, worldBoundsInset.Bottom), 0.2f, Ease.QuadFastSlow);

        main.AddSpawnEvent(rightMiddleSpawn)
            .AddMoveTo(new Vector2(worldBounds.Width * 7 / 8f, worldBoundsInset.Bottom), 0.2f, Ease.QuadFastSlow);

        main.AddWaitEvent(0.1f);
        main.AddSpawnEvent(leftMiddleSpawn)
            .AddMoveTo(new Vector2(worldBounds.Width / 3, worldBoundsInset.Bottom), 0.2f, Ease.QuadFastSlow);
        main.AddSpawnEvent(rightMiddleSpawn)
            .AddMoveTo(new Vector2(worldBounds.Width * 2 / 3f, worldBoundsInset.Bottom), 0.2f, Ease.QuadFastSlow);

        yield return summon;
    }
}