﻿using System;

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

    public float BossMeter { get; private set; }

    public void UpdateBossMeter(float dt)
    {
        if (MathF.Abs(IntensityAsBidirectionalPercent) < 0.25f)
        {
            BossMeter += dt / 10;
        }
    }

    public int Health;

    public int Bombs;

    public PowerUpType? CurrentPowerUp;
}
