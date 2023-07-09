using System;

namespace GMTK23;

public class PlayerStatistics
{
    private float _intensity = -75;

    public float Intensity
    {
        get => _intensity;
        set => _intensity = Math.Clamp(value, -100, 100);
    }

    public float IntensityAsBidirectionalPercent => Intensity / 100f;

    public float BossMeter { get; set; }
    public bool SpawnedBoss { get; set; }
    
    public int Level { get; set; }

    public void UpdateBossMeter(float dt, bool hasEnemies)
    {
        if (MathF.Abs(IntensityAsBidirectionalPercent) < 0.25f &&  hasEnemies && Health > 0)
        {
            var increment = dt / Math.Max(5, 10 - Level * 2f);

            if(Global.IsFtue)
            {
                // boss meter does not update on first time play
                return;
            }

            if (Level == 0)
            {
                // boss meter takes twice as long to raise on level 0, player is supposed to fail on the first go
                increment /= 2f;
            }
            BossMeter += increment;
        }
    }

    public int Health;

    public int Bombs;

    public PowerUpType? CurrentPowerUp;
}
