using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class RectVfx : Entity
{
    public SequenceTween Tween { get; } = new();
    public TweenableVector2 TweenableSize { get; } = new(Vector2.Zero);
    public TweenableFloat TweenableAngle { get; } = new(0);
    public TweenableFloat TweenableOpacity { get; } = new(1);

    public override void Draw(Painter painter)
    {
        painter.DrawRectangle(
            new RectangleF(Position, TweenableSize),
            new DrawSettings
                {Angle = TweenableAngle, Color = Color.White.WithMultipliedOpacity(TweenableOpacity), Origin = DrawOrigin.Center});
    }

    public override void Update(float dt)
    {
        Tween.Update(dt);

        if (Tween.IsDone())
        {
            Destroy();
        }
    }
}
