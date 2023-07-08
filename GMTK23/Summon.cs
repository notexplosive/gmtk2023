using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Summon
{
    private readonly Game _game;
    private readonly SummonStats _summonStats;
    public float Cooldown { get; }
    private readonly List<MainChoreoid> _choreiods = new();

    public Summon(Game game, SummonStats summonStats)
    {
        _game = game;
        _summonStats = summonStats;
        Cooldown = summonStats.Cooldown;
    }

    public void Execute()
    {
        foreach (var choreoid in _choreiods)
        {
            _game.ActiveTween.AddChannel(choreoid.GenerateTween());
        }
    }

    public void SpawnShipAt(ShipStats stats, Vector2 position, ShipChoreoid shipChoreoid)
    {
        var enemyShip = _game.World.Entities.AddImmediate(new EnemyShip(stats, shipChoreoid));
        enemyShip.Position = position;
    }

    public MainChoreoid AddChoreoid(ShipStats stats)
    {
        var choreoid = new MainChoreoid(this, stats);
        _choreiods.Add(choreoid);
        return choreoid;
    }
}

public record SummonStats(float Cooldown);
