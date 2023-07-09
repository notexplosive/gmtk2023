using System.Collections.Generic;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public static class ScriptContent
{
    public static BulletStats PlayerBullet => new(10, 34, "gmtk23_shoot1");
    public static BulletStats TripleShotBullet => new(10, 34, "gmtk23_shoot1", Cooldown: 0.3f);
    public static BulletStats PlayerBulletHoming => new(10, 34, "gmtk23_shoot2", PowerUpType.HomingShot, Cooldown: 0.3f);
    public static BulletStats PlayerBulletPiercing => new(10, 34, "gmtk23_shoot7", PowerUpType.PiercingShot);

    public static BulletStats NoBullet => new(0, 35);
    public static BulletStats BasicEnemyBullet => new(3, 35);
    public static BulletStats Stinger => new(5, 36);
    public static BulletStats SlowVenom => new(2, 37);
    public static BulletStats FastVenom => new(5, 37);
    public static ShipStats BasicEnemy => new(1, 2, new Vector2(10), ScriptContent.BasicEnemyBullet with {Cooldown = 3}, 1f);
    public static ShipStats PillBug => new(8, 5, new Vector2(10), ScriptContent.Stinger with {Cooldown = 3}, 1f);
    public static ShipStats BigCricket => new(13, 5, new Vector2(10), ScriptContent.FastVenom with {Cooldown = 3}, 1f);
    public static ShipStats Beetle => new(2, 20, new Vector2(10), ScriptContent.Stinger with {Cooldown = 1}, 1f);
    public static ShipStats SmallCicada => new(9, 3, new Vector2(10), ScriptContent.BasicEnemyBullet  with {Cooldown = 5}, 1f);
    public static ShipStats SmallCricket => new(3, 1, new Vector2(5), ScriptContent.NoBullet, 1f);

    public static ShipStats Centipede => new(10, 15, new Vector2(10), ScriptContent.SlowVenom with {Cooldown = 1}, 1f,
        new TailStats(11, 5, ScriptContent.SlowVenom with {Cooldown = 1}, 2));

    public static IEnumerable<Wave> Summons(Game game)
    {
        yield return ScriptContent.BasicFormation(game);
        yield return ScriptContent.BeetleTankWave(game);
        yield return ScriptContent.CentipedeWave(game);
        yield return ScriptContent.SmallCicadaWave(game);
        yield return ScriptContent.CricketSwarmWave(game);
        yield return ScriptContent.PillBugWave(game);
        yield return ScriptContent.BigCricketWave(game);
    }

    private static Wave CricketSwarmWave(Game game)
    {
        var worldBounds = game.World.Bounds;
        var center = worldBounds.Center.X;
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var worldBoundsInset = worldBounds.Inflated(-100, -100);

        var wave = new Wave(game, new WaveStats(10f,2));
        var leftSpawner = wave.AddChoreoid(ScriptContent.SmallCricket);
        var rightSpawner = wave.AddChoreoid(ScriptContent.SmallCricket);

        var leftShips = new List<ShipChoreoid>();
        var rightShips = new List<ShipChoreoid>();
        
        for (int i = 0; i < 10; i++)
        {
            var leftShip = leftSpawner.AddSpawnEvent(new Vector2(worldBoundsOutset.Left, worldBoundsInset.Bottom));
            leftSpawner.AddWaitEvent(0.25f);
            leftShip.AddMoveToFastX(new Vector2(center, worldBoundsOutset.Bottom), 1);
            leftShips.Add(leftShip);
            
            var rightShip = rightSpawner.AddSpawnEvent(new Vector2(worldBoundsOutset.Right, worldBoundsInset.Bottom));
            rightSpawner.AddWaitEvent(0.25f);
            rightShip.AddMoveToFastX(new Vector2(center, worldBoundsOutset.Bottom), 1);
            rightShips.Add(rightShip);
        }

        {
            var pos = worldBoundsInset.BottomLeft;
            var increment = worldBoundsInset.Width / leftShips.Count;
            foreach (var ship in leftShips)
            {
                ship.AddMoveToFastY(pos, 0.5f);
                pos.X += increment;
            }
        }
        
        {
            var pos = worldBoundsInset.BottomRight;
            var increment = worldBoundsInset.Width / rightShips.Count;
            foreach (var ship in rightShips)
            {
                ship.AddMoveToFastY(pos, 0.5f);
                pos.X -= increment;
            }
        }

        return wave;
    }

    private static Wave SmallCicadaWave(Game game)
    {
        var worldBounds = game.World.Bounds;
        var worldBoundsInset = worldBounds.Inflated(-100, -100);
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var center = worldBounds.Center.X;

        var wave = new Wave(game, new WaveStats(5f, 10));

        var main = wave.AddChoreoid(ScriptContent.SmallCicada);

        var a = main.AddSpawnEvent(new Vector2(center, worldBounds.Bottom));
        var b = main.AddSpawnEvent(new Vector2(center, worldBounds.Bottom));

        var beatLength = 0.1f;
        var duration = 0.5f;
        
        void ShootBurst(ShipChoreoid choreoid)
        {
            choreoid
                .AddShoot()
                .AddWait(beatLength)
                .AddShoot()
                .AddWait(beatLength);
        }

        a.AddMoveToFastX(new Vector2(worldBoundsInset.Right, center), duration).AddWait(beatLength);
        b.AddMoveToFastX(new Vector2(worldBoundsInset.Left, center), duration).AddWait(beatLength);

        // Swap
        a.AddMoveToFastY(new Vector2(worldBoundsInset.Right, worldBoundsInset.Bottom), duration).AddWait(beatLength);
        b.AddMoveToFastY(new Vector2(worldBoundsInset.Left, worldBoundsInset.Bottom), duration).AddWait(beatLength);
        
        // Shoot
        ShootBurst(a);
        ShootBurst(b);
        
        // Swap
        worldBoundsInset = worldBoundsInset.Inflated(-20, 0);
        b.AddMoveToFastX(new Vector2(worldBoundsInset.Right, center), duration).AddWait(beatLength);
        a.AddMoveToFastX(new Vector2(worldBoundsInset.Left, center), duration).AddWait(beatLength);
        
        // Shoot
        ShootBurst(a);
        ShootBurst(b);
        
        // move back
        worldBoundsInset = worldBoundsInset.Inflated(-20, 0);
        a.AddMoveToFastY(new Vector2(worldBoundsInset.Right, worldBoundsInset.Bottom), duration).AddWait(beatLength);
        b.AddMoveToFastY(new Vector2(worldBoundsInset.Left, worldBoundsInset.Bottom), duration).AddWait(beatLength);
        
        // Shoot
        ShootBurst(a);
        ShootBurst(b);

        return wave;
    }

    private static Wave CentipedeWave(Game game)
    {
        var worldBounds = game.World.Bounds;
        var center = worldBounds.Center.X;

        var wave = new Wave(game, new WaveStats(10f, 9));

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
            ship.PlaySound("gmtk23_repeat1");

            // across
            ship.MoveLinear(new Vector2(worldBounds.Right, worldBounds.Bottom - forwardStep), 0.5f);
            forwardStep += 20f;

            // forward (fast)
            ship.MoveLinear(new Vector2(worldBounds.Right, worldBounds.Bottom - forwardStep), 0.05f);
            ship.PlaySound("gmtk23_repeat1");

        }

        return wave;
    }

    private static Wave BeetleTankWave(Game game)
    {
        var worldBounds = game.World.Bounds;
        var worldBoundsInset = worldBounds.Inflated(-50, -50);
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var center = worldBounds.Center.X;

        var wave = new Wave(game, new WaveStats(5f, 1));

        var main = wave.AddChoreoid(ScriptContent.Beetle);

        var ship = main.AddSpawnEvent(new Vector2(center, worldBounds.Bottom))
            .AddMoveToFastX(new Vector2(center, center), 0.5f)
            .AddWait(0.1f);

        var backStep = 0f;
        var moveDuration = 3f;
        for (var i = 0; i < 5; i++)
        {
            backStep += 10f;
            ship.AddMoveToFastX(new Vector2(worldBounds.Width / 8, center + backStep), moveDuration)
                .AddWait(0.5f)
                .AddShoot()
                .AddWait(0.5f);
            backStep += 10f;
            ship.AddMoveToFastX(new Vector2(worldBounds.Width * 7 / 8, center + backStep), moveDuration)
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
        var wave = new Wave(game, new WaveStats(1f, 0));

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
    
    private static Wave PillBugWave(Game game)
    {
        var worldBounds = game.World.Bounds;
        var worldBoundsInset = worldBounds.Inflated(-20, -20);
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var wave = new Wave(game, new WaveStats(6f, 4));

        var main = wave.AddChoreoid(ScriptContent.PillBug);

        int numberOfSpawns = 5;
        for (int i = 0; i < numberOfSpawns + 1; i++)
        {
            var percent = (float)i / numberOfSpawns;
            main.AddSpawnEvent(Vector2Extensions.Lerp(new Vector2(worldBoundsInset.Left,worldBoundsOutset.Bottom), new Vector2(worldBoundsInset.Right,worldBoundsOutset.Bottom),
                percent));
        }

        return wave;
    }
    
    private static Wave BigCricketWave(Game game)
    {
        var worldBounds = game.World.Bounds;
        var worldBoundsInset = worldBounds.Inflated(-20, -20);
        var worldBoundsOutset = worldBounds.Inflated(64, 64);
        var wave = new Wave(game, new WaveStats(6f, 7));

        var main = wave.AddChoreoid(ScriptContent.BigCricket);

        worldBoundsInset = RectangleF.Intersect(worldBoundsInset.Moved(new Vector2(0, 100)), worldBounds);

        int numberOfSpawns = 3;
        for (int i = 0; i < numberOfSpawns; i++)
        {
            var percent = (float)i / numberOfSpawns;
            var ship = main.AddSpawnEvent(Vector2Extensions.Lerp(worldBoundsOutset.BottomLeft, worldBoundsOutset.BottomRight,
                percent));


            for (int j = 0; j < 10; j++)
            {
                var randX = Client.Random.Clean.NextFloat();
                var randY = Client.Random.Clean.NextFloat();
                ship.AddMoveToFastX(worldBoundsInset.TopLeft + worldBoundsInset.Size.StraightMultiply(randX, randY),
                    1f);
                ship.AddWait(0.5f);
                ship.AddShoot();
                ship.AddWait(0.2f);
                ship.AddShoot();
                ship.AddWait(0.2f);
                ship.AddShoot();
                ship.AddWait(0.2f);
            }
        }

        return wave;
    }

    public static IEnumerable<PlayerPersonality> PlayerPersonalities()
    {
        
        
        yield return new PlayerPersonality
        {
            // default
        };
        
        yield return new PlayerPersonality
        {
            RiskTolerance = 1f,
            ComfortZone = new (Vector2.Zero, new Vector2(420, 420)),
            ShootReactionSkillPercent = 1f
        };
        
        yield return new PlayerPersonality
        {
            RiskTolerance = 0.2f,
            ComfortZone = new (Vector2.Zero, new Vector2(420, 420)),
            ShootReactionSkillPercent = 0.5f
        };
        
        yield return new PlayerPersonality
        {
            ComfortZone = RectangleF.FromCorners(new Vector2(0,100), new Vector2(420, 110)),
            ShootReactionSkillPercent = 0.1f
        };

    }
}

