namespace GMTK23;

public record PlayerStateArgs(float DeltaTime, PlayerShip Player, World World);

public abstract class PlayerState
{
    public PlayerState UpdateState(float dt, PlayerShip player, World world)
    {
        var args = new PlayerStateArgs(dt, player, world);
        UpdateInternal(args);
        
        if (IsFinished(player, world))
        {
            return GetNextState(player, world);
        }

        return this;
    }

    protected abstract void UpdateInternal(PlayerStateArgs args);
    public abstract PlayerState GetNextState(PlayerShip player, World world);
    public abstract bool IsFinished(PlayerShip player, World world);
}