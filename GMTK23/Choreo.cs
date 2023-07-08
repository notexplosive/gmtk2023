using System;
using System.Collections.Generic;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public interface IChoreoid
{
}

public class MainChoreoid : IChoreoid
{
    private readonly ShipStats _shipStats;
    private readonly Summon _summon;

    /// <summary>
    ///     This needs tobe a list of Tween GENERATORS not ITweens directly because we need copies
    /// </summary>
    private readonly List<Func<ITween>> _tweenInstructions = new();

    public MainChoreoid(Summon summon, ShipStats shipStats)
    {
        _summon = summon;
        _shipStats = shipStats;
    }

    public MainChoreoid AddWaitEvent(float seconds)
    {
        _tweenInstructions.Add(() => new WaitSecondsTween(seconds));
        return this;
    }

    public ShipChoreoid AddSpawnEvent(Vector2 startPosition)
    {
        var shipChoreoid = new ShipChoreoid();
        _tweenInstructions.Add(
            () => new CallbackTween(
                () => _summon.SpawnShipAt(_shipStats, startPosition, shipChoreoid))
            );
        return shipChoreoid;
    }

    public MainChoreoid Add(Func<ITween> tween)
    {
        _tweenInstructions.Add(tween);
        return this;
    }

    public SequenceTween GenerateTween()
    {
        var sequence = new SequenceTween();

        foreach (var tweenGeneratorFunction in _tweenInstructions)
        {
            sequence.Add(tweenGeneratorFunction());
        }

        return sequence;
    }
}
