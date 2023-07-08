using System;
using System.Collections.Generic;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class ShipChoreoid : IChoreoid
{
    public delegate ITween ShipChoreoidDelegate(TweenableFloat x, TweenableFloat y);
    private readonly List<ShipChoreoidDelegate> _tweenInstructions = new();

    public SequenceTween GenerateTween(TweenableFloat shipX,TweenableFloat shipY)
    {
        var result = new SequenceTween();
        foreach (var instruction in _tweenInstructions)
        {
            result.Add(instruction(shipX, shipY));
        }

        return result;
    }

    public void AddCallback(Action callback)
    {
        Add((x,y) => new CallbackTween(callback));
    }

    public ShipChoreoid AddMoveTo(Vector2 target, float duration, Ease.Delegate easeFunction)
    {
        
        Add((x,y) => new MultiplexTween()
            .AddChannel(x.TweenTo(target.X, duration, easeFunction))
            .AddChannel(y.TweenTo(target.Y, duration, easeFunction)));
        return this;
    }

    public void Add(ShipChoreoidDelegate tween)
    {
        _tweenInstructions.Add(tween);
    }
}
