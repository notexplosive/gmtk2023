using ExplogineMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class HuntingState : PlayerState
{
    private readonly EnemyShip _target;

    public HuntingState(EnemyShip target)
    {
        _target = target;
    }

    protected override void UpdateInternal(PlayerStateArgs args)
    {
        args.Player.TargetPosition = new Vector2(_target.Position.X, args.Player.TargetPosition.Y);
    }

    public override PlayerState GetNextState(PlayerShip player, World world)
    {
        return IdleState.Instance;
    }

    public override bool IsFinished(PlayerShip player, World world)
    {
        return _target.IsDead;
    }
}
