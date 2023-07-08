using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Bullet : Entity
{
    private readonly BulletStats _bulletStats;
    private readonly float _speed;

    public Bullet(Team team, BulletStats bulletStats)
    {
        _bulletStats = bulletStats;
        Team = team;
        _speed = bulletStats.Speed;
        Size = new Vector2(5);
    }

    public Team Team { get; }
    public RectangleF DealDamageBox => BoundingBox;

    public override void Draw(Painter painter)
    {
        painter.DrawRectangle(BoundingBox, new DrawSettings {Depth = RenderDepth});
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
