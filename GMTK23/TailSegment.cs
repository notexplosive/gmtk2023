using System;
using System.Collections.Generic;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class TailSegment : TeamedEntity
{
    private readonly int _frame;
    private readonly EnemyShip _master;
    private readonly Queue<Vector2> _nextPositions;
    private readonly Entity _nextSegment;

    public override RectangleF TakeDamageBox => BoundingBox;

    public TailSegment(Entity nextSegment, EnemyShip master, TailStats tailStats) : base(master.Team)
    {
        _nextSegment = nextSegment;
        _master = master;

        _frame = tailStats.Frame;
        master.Destroyed += () =>
        {
            World.Entities.AddImmediate(new Vfx(Client.Assets.GetAsset<GridBasedSpriteSheet>("Explosion"), Position,
                0.5f));
            World.Entities.RemoveImmediate(this);
        };

        Position = nextSegment.Position;
        _nextPositions = new Queue<Vector2>();
        _nextPositions.Enqueue(nextSegment.Position);
        _nextPositions.Enqueue(nextSegment.Position);
    }

    public override void Draw(Painter painter)
    {
        var nextPosition = _nextPositions.Peek();

        var sheet = Global.MainSheet;

        if (_master.DamageFlashTimer > 0)
        {
            sheet = Global.MainSheetWithFlash;
        }

        var angle = (nextPosition - Position).GetAngleFromUnitX() - MathF.PI / 2;

        sheet.DrawFrameAtPosition(painter, _frame, Position, Scale2D.One,
            new DrawSettings {Origin = DrawOrigin.Center, Depth = RenderDepth, Angle = angle});
    }

    public override void Update(float dt)
    {
        Position = _nextPositions.Dequeue();
        _nextPositions.Enqueue(_nextSegment.Position);
    }

    protected override void TakeDamageInternal()
    {
        _master.TakeDamage();
    }
}
