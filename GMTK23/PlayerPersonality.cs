using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

[Serializable]
public class PlayerPersonality
{
    public float MovementReactionSkillPercent()
    {
        // how quick they are to react (with regard to movement)
        return 0.20f;
    }

    public float Clumsiness()
    {
        // How much they fudge their target position
        return 20;
    }

    public float HowCloseItWantsToBeToTargetPosition()
    {
        // how close (in pixels) until it's "close enough" to it's target position
        return 30;
    }

    public float ShootReactionSkillPercent { get; set; } = 0.30f;

    public float RiskTolerance { get; set; } = 0.1f;

    // the zone in which the player is comfortable moving around, they will only leave this zone if they do so by accident
    // their AI only checks within the comfort zone for calculations
    public RectangleF ComfortZone { get; set; } = new (Vector2.Zero, new Vector2(420, 420 / 3f));

    public RectangleF PreferredZone(Vector2 worldSize)
    {
        return ComfortZone.Inflated(-40, -40);
    }
}
