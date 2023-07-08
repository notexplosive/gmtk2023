using System.Linq;
using ExplogineMonoGame;

namespace GMTK23;

public class IdleState : PlayerState
{
    public static readonly IdleState Instance = new();

    private IdleState()
    {
    }

    public override bool IsFinished(PlayerShip player, World world)
    {
        return true;
    }

    protected override void UpdateInternal(PlayerStateArgs args)
    {
    }

    public override PlayerState GetNextState(PlayerShip player, World world)
    {
        // todo: check if should be overwhelmed

        // hunt for another enemy to kill
        var enemies = player.Enemies.ToList();
        if (enemies.Count > 0)
        {
            var target = Client.Random.Clean.GetRandomElement(enemies);
            return new HuntingState(target);
        }

        return this;
    }
}
