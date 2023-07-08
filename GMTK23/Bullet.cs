using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Bullet : Entity
{
    private readonly float _speed;

    public Bullet(Team team, float speed)
    {
        Team = team;
        _speed = speed;
        Size = new Vector2(5);
    }

    public Team Team { get; }

    public override void Draw(Painter painter)
    {
        painter.DrawRectangle(HitBox, new DrawSettings {Depth = RenderDepth});
    }

    public override void Update(float dt)
    {
        var velocity = dt * new Vector2(0, _speed * 60f);

        if (Team == Team.Enemy)
        {
            velocity.Y = -velocity.Y;
        }

        Position += velocity;
        DestroyIfOutOfBounds();
    }
}

public enum Team
{
    Enemy,
    Player
}
