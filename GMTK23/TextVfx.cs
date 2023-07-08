using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class TextVfx : Entity
{
    private readonly string _text;
    public SequenceTween Tween { get; } = new();
    public TweenableVector2 TweenablePositionOffset { get; } = new(Vector2.Zero);

    public TextVfx(string text)
    {
        _text = text;

        Tween.Add(TweenablePositionOffset.TweenTo(new Vector2(0, -50), 0.25f, Ease.CubicFastSlow));
        Tween.Add(new WaitSecondsTween(0.25f));
    }
    
    public override void Draw(Painter painter)
    {
        painter.DrawStringWithinRectangle(
            Client.Assets.GetFont("gmtk/GameFont", 16),
            _text,
            RectangleF.InflateFrom(Position + TweenablePositionOffset, 500, 500), Alignment.Center, new DrawSettings()
        );
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
