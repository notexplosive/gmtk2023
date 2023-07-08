﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GMTK23;

public static class ScriptContent
{
    public static BulletStats PlayerBullet => new(10, 34);

    public static BulletStats BasicEnemyBullet => new(3, 35);
    public static BulletStats Stinger => new(5, 36);
    public static BulletStats SlowVenom => new(1, 37);
    public static ShipStats BasicEnemy => new(1, 2, 3, new Vector2(10), ScriptContent.BasicEnemyBullet, 1f);
    public static ShipStats Beetle => new(2, 20, 1, new Vector2(10), ScriptContent.Stinger, 1f);
    public static ShipStats Centipede => new(10, 20, 1, new Vector2(10), ScriptContent.SlowVenom, 1f, new TailStats(11, 5));

    public static IEnumerable<Wave> Summons(Game game)
    {
        yield return ScriptContent.BasicFormation(game);
        yield return ScriptContent.BeetleTank(game);
        yield return ScriptContent.CentipedeWave(game);
    }

    private static Wave CentipedeWave(Game game)
    {
        var worldBounds = game.World.Bounds;
        var center = worldBounds.Center.X;

        var wave = new Wave(game, new WaveStats(10f));
        
        var main = wave.AddChoreoid(ScriptContent.Centipede);

        var ship = main.AddSpawnEvent(new Vector2(worldBounds.Right, worldBounds.Bottom));
        
        var forwardStep = 0f;
        for (var i = 0; i < 15; i++)
        {
            // across
            ship.MoveLinear(new Vector2(worldBounds.Left, worldBounds.Bottom - forwardStep), 0.5f);
            forwardStep += 20f;
            
            // forward (fast)
            ship.MoveLinear(new Vector2(worldBounds.Left, worldBounds.Bottom - forwardStep), 0.05f);
            
            // across
            ship.MoveLinear(new Vector2(worldBounds.Right, worldBounds.Bottom - forwardStep), 0.5f);
            forwardStep += 20f;
            
            // forward (fast)
            ship.MoveLinear(new Vector2(worldBounds.Right, worldBounds.Bottom - forwardStep), 0.05f);
        }

        return wave;
    }

    private static Wave BeetleTank(Game game)
    {
        var worldBounds = game.World.Bounds;
        var worldBoundsInset = worldBounds.Inflated(-50, -50);
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var center = worldBounds.Center.X;

        var wave = new Wave(game, new WaveStats(5f));

        var main = wave.AddChoreoid(ScriptContent.Beetle);

        var ship = main.AddSpawnEvent(new Vector2(center, worldBounds.Bottom))
            .AddMoveToFastX(new Vector2(center, center), 0.5f)
            .AddWait(0.1f);

        var backStep = 0f;
        for (var i = 0; i < 5; i++)
        {
            backStep += 10f;
            ship.AddMoveToFastX(new Vector2(worldBounds.Width / 8, center + backStep), 0.5f)
                .AddWait(0.5f)
                .AddShoot()
                .AddWait(0.5f);
            backStep += 10f;
            ship.AddMoveToFastX(new Vector2(worldBounds.Width * 7 / 8, center + backStep), 0.5f)
                .AddWait(0.5f)
                .AddShoot()
                .AddWait(0.5f);
        }

        ship.AddMoveToFastX(new Vector2(center, worldBoundsInset.Bottom), 0.5f);

        return wave;
    }

    private static Wave BasicFormation(Game game)
    {
        var worldBounds = game.World.Bounds;
        var worldBoundsInset = worldBounds.Inflated(-20, -20);
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var wave = new Wave(game, new WaveStats(1f));

        var main = wave.AddChoreoid(ScriptContent.BasicEnemy);

        var rightMiddleSpawn = new Vector2(worldBoundsOutset.Right, worldBounds.Center.Y);
        var leftMiddleSpawn = new Vector2(worldBoundsOutset.Left, worldBounds.Center.Y);

        main.AddSpawnEvent(leftMiddleSpawn)
            .AddMoveToFastX(new Vector2(worldBounds.Width / 8, worldBoundsInset.Bottom), 0.5f)
            .AddWait(0.1f);
        main.AddSpawnEvent(rightMiddleSpawn)
            .AddMoveToFastX(new Vector2(worldBounds.Width * 7 / 8f, worldBoundsInset.Bottom), 0.5f)
            .AddWait(0.1f);

        main.AddWaitEvent(0.2f);
        main.AddSpawnEvent(leftMiddleSpawn)
            .AddMoveToFastX(new Vector2(worldBounds.Width / 3, worldBoundsInset.Bottom), 0.5f)
            .AddWait(0.1f);
        main.AddSpawnEvent(rightMiddleSpawn)
            .AddMoveToFastX(new Vector2(worldBounds.Width * 2 / 3f, worldBoundsInset.Bottom), 0.5f)
            .AddWait(0.1f);

        return wave;
    }
}
