using System;

namespace GMTK23;

public class PlayerStatistics
{
    private float _intensity = 0;

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
            BossMeter += dt / Math.Max(5, 10 - Level);
        }
    }

    public int Health;

    public int Bombs;

    public PowerUpType? CurrentPowerUp;
}
