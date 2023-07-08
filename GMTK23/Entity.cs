using System;
using System.Collections.Generic;
using System.Linq;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public abstract class Entity : IUpdateHook
{
    private bool _hasEnteredBounds;

    public Entity()
    {
        RenderDepth = Depth.Middle;
    }

    public Vector2 Position { get; set; }
    public Depth RenderDepth { get; set; }
    public World World { get; set; } = null!;

    public IEnumerable<Entity> OtherEntities => World.Entities.Where(entity => entity != this);
    public IEnumerable<TeamedEntity> Enemies => OtherEntities.OfType<TeamedEntity>();
    public IEnumerable<Bullet> Bullets => OtherEntities.OfType<Bullet>();
    public bool FlaggedAsDead { get; set; }
    public bool IsAlive => !FlaggedAsDead;
    public bool IsDead => FlaggedAsDead;
    public RectangleF BoundingBox => new RectangleF(Position, Vector2.Zero).Inflated(Size.X / 2f, Size.Y / 2f);
    public Vector2 Size { get; set; }

    public virtual void Update(float dt)
    {
        // blank on purpose
    }

    public virtual void Awake()
    {
        // blank on purpose
    }

    public abstract void Draw(Painter painter);

    public void DestroyIfOutOfBounds()
    {
        var inBounds = World.Bounds.Inflated(32, 32).Contains(Position);

        if (inBounds)
        {
            _hasEnteredBounds = true;
        }

        if (_hasEnteredBounds)
        {
            if (!inBounds)
            {
                Destroy();
            }
        }
    }

    public void Destroy()
    {
        World.Entities.DeferredActions.Add(() =>
        {
            World.Entities.RemoveImmediate(this);
            Destroyed?.Invoke();
        });
    }

    public event Action? Destroyed;
}
