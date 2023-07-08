using System;
using System.Collections.Generic;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class ShipChoreoid : IChoreoid
{
    public delegate ITween ShipChoreoidDelegate(TweenableVector2 position);
    private readonly List<ShipChoreoidDelegate> _tweenInstructions = new();

    public SequenceTween GenerateTween(TweenableVector2 shipPosition)
    {
        var result = new SequenceTween();
        foreach (var instruction in _tweenInstructions)
        {
            result.Add(instruction(shipPosition));
        }

        return result;
    }

    public void AddCallback(Action callback)
    {
        Add(pos => new CallbackTween(callback));
    }

    public ShipChoreoid AddMoveTo(Vector2 target, float duration, Ease.Delegate easeFunction)
    {
        Add(pos => pos.TweenTo(target, 0.5f, Ease.Linear));
        return this;
    }

    public void Add(ShipChoreoidDelegate tween)
    {
        _tweenInstructions.Add(tween);
    }
}
