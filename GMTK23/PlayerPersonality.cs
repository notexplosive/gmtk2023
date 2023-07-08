using System;
using ExplogineMonoGame.Data;
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

    public RectangleF ComfortZone(Vector2 worldSize)
    {
        // the zone in which the player is comfortable moving around, they will only leave this zone if they do so by accident
        // their AI only checks within the comfort zone for calculations
        return new RectangleF(Vector2.Zero, new Vector2(worldSize.X, worldSize.Y / 3));
    }

    public RectangleF PreferredZone(Vector2 worldSize)
    {
        return ComfortZone(worldSize).Inflated(-40, -40);
    }
}
