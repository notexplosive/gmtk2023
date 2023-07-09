using System;
using ExplogineMonoGame;

namespace GMTK23;

public class Boss : Ship
{
    public Boss(Team team, int health) : base(team, health)
    {
    }

    public override void Draw(Painter painter)
    {
        
    }

    public override bool HasInvulnerabilityFrames()
    {
        throw new NotImplementedException();
    }
}
