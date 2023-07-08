using System;
using Microsoft.Xna.Framework;

namespace GMTK23;

[Serializable]
public class PlayerPersonality
{
    public float PersonalSpaceRadius()
    {
        return 100;
    }

    public float MovementReactionSkillPercent()
    {
        return 0.30f;
    }

    public float Clumsiness()
    {
        // clumsy is (1f - accuracy)
        return 20;
    }

    public float HowCloseItWantsToBeToTargetPosition()
    {
        // how close (in pixels) until it's "close enough" to it's target position
        return 30;
    }

    public float ShootReactionSkillPercent()
    {
        return 0.30f;
    }
}
