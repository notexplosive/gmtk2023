using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Wave
{
    private readonly Game _game;
    private readonly WaveStats _waveStats;
    public float Cooldown { get; }
    private readonly List<MainChoreoid> _choreiods = new();

    public Wave(Game game, WaveStats waveStats)
    {
        _game = game;
        _waveStats = waveStats;
        Cooldown = waveStats.Cooldown;
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

public record WaveStats(float Cooldown);
