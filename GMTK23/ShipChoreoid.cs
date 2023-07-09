using System;
using System.Collections.Generic;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class ShipChoreoid : IChoreoid
{
    public delegate ITween ShipChoreoidDelegate(EnemyShip ship, TweenableFloat x, TweenableFloat y);

    private readonly List<ShipChoreoidDelegate> _tweenInstructions = new();

    public SequenceTween GenerateTween(EnemyShip ship, TweenableFloat shipX, TweenableFloat shipY)
    {
        var result = new SequenceTween();
        foreach (var instruction in _tweenInstructions)
        {
            result.Add(new DynamicTween(() => instruction(ship, shipX, shipY)));
        }

        return result;
    }

    public void AddCallback(Action callback)
    {
        Add((ship, x, y) => new CallbackTween(callback));
    }

    public void PlaySound(string soundName)
    {
        AddCallback(() => Global.PlaySound(soundName));
    }

    public ShipChoreoid AddMoveTo(Vector2 target, float duration, Ease.Delegate easeFunction)
    {
        return AddMoveTo(target, duration, easeFunction, easeFunction);
    }

    public ShipChoreoid AddMoveTo(Vector2 target, float duration, Ease.Delegate easeFunctionX,
        Ease.Delegate easeFunctionY, int shotsDuring = 0)
    {
        
        
        
        Add((ship, x, y) =>
        {
            var shootingTween = new SequenceTween();

            for (int i = 0; i < shotsDuring; i++)
            {
                shootingTween.Add(new CallbackTween(ship.ShootPreferredBullet));
                shootingTween.Add(new WaitSecondsTween(duration / shotsDuring));
            }

            return new MultiplexTween()
                .AddChannel(x.TweenTo(target.X, duration, easeFunctionX))
                .AddChannel(y.TweenTo(target.Y, duration, easeFunctionY))
                .AddChannel(shootingTween);
        });
        return this;
    }

    public ShipChoreoid MoveLinear(Vector2 target, float duration)
    {
        return AddMoveTo(target, duration, Ease.Linear, Ease.Linear);
    }

    public ShipChoreoid AddMoveToFastX(Vector2 target, float duration, int shotsDuring = 0)
    {
        return AddMoveTo(target, duration, Ease.QuadFastSlow, Ease.QuadSlowFast, shotsDuring);
    }

    public ShipChoreoid AddMoveToFastY(Vector2 target, float duration)
    {
        return AddMoveTo(target, duration, Ease.QuadSlowFast, Ease.QuadFastSlow);
    }

    public void Add(ShipChoreoidDelegate tween)
    {
        _tweenInstructions.Add(tween);
    }

    public ShipChoreoid AddWait(float duration)
    {
        Add((ship, x, y) => new WaitSecondsTween(duration));
        return this;
    }

    public ShipChoreoid AddShoot()
    {
        Add((ship, x, y) => new CallbackTween(ship.ShootPreferredBullet));
        return this;
    }
}
