using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Summon
{
    private readonly Game _game;
    private readonly SummonStats _summonStats;
    public float Cooldown { get; }
    private List<ShipSpawn> _shipSpawns = new();

    public Summon(Game game, SummonStats summonStats)
    {
        _game = game;
        _summonStats = summonStats;
        Cooldown = summonStats.Cooldown;
    }

    public void Execute()
    {
        foreach (var spawn in _shipSpawns)
        {
            var enemyShip = _game.World.Entities.AddImmediate(new EnemyShip(spawn.Stats));
            enemyShip.Position = spawn.Position;
        }
    }

    public void SpawnShipAt(ShipStats stats, Vector2 vector2)
    {
        _shipSpawns.Add(new ShipSpawn(stats,vector2));
    }
}

public record SummonStats(float Cooldown);
