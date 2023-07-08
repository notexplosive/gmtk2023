using System.Collections.Generic;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public static class ScriptContent
{
    public static BulletStats PlayerBullet => new(10, 34);

    public static BulletStats BasicEnemyBullet => new(3, 35);
    public static BulletStats Stinger => new(5, 36);
    public static ShipStats BasicEnemy => new(1, 2, 3, new Vector2(10), ScriptContent.BasicEnemyBullet, 1f);
    public static ShipStats Beetle => new(2, 20, 1, new Vector2(10), ScriptContent.Stinger, 1f);

    public static IEnumerable<Wave> Summons(Game game)
    {
        yield return BasicFormation(game);
        yield return Tank(game);
    }

    private static Wave Tank(Game game)
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
        for (int i = 0; i < 5; i++)
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
