using System.Linq;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class StatusScreen : Widget, IEarlyDrawHook, IUpdateHook
{
    private readonly Game _game;
    private readonly SequenceTween _tween;
    private readonly TweenableFloat _lerpPercent = new(0);

    public StatusScreen(RectangleF windowRectangle, Game game) : base(windowRectangle, Depth.Middle)
    {
        _game = game;
        _tween = new SequenceTween();
        _tween.Add(_lerpPercent.TweenTo(1f, 1, Ease.Linear));
        _tween.Add(_lerpPercent.TweenTo(0, 1, Ease.Linear));
        _tween.IsLooping = true;
    }

    public void EarlyDraw(Painter painter)
    {

        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch();

        var bodyRect = ContentRectangle.MovedToZero();

        painter.DrawRectangle(bodyRect,
            new DrawSettings {Depth = Depth.Middle + 50, Color = Color.White.DimmedBy(0.9f)});
        var percent = _game.World.PlayerStatistics.BossMeter;
        painter.DrawRectangle(
            RectangleF.FromCorners(bodyRect.BottomLeft,
                Vector2Extensions.Lerp(bodyRect.BottomRight, bodyRect.TopRight, percent)),
            new DrawSettings
                {Depth = Depth.Middle, Color = ColorExtensions.Lerp(Color.Orange, Color.OrangeRed, _lerpPercent)});

        if (!Global.IsFtue)
        {
            var text = "BOSS METER";

            if (_game.World.PlayerStatistics.SpawnedBoss)
            {
                text = "COMPLETE!";
                if (_game.World.Entities.Any(e => e is Boss))
                {
                    text = "BOSS DEPLOYED";
                }
            }

            painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), text, bodyRect,
                Alignment.Center, new DrawSettings {Depth = Depth.Middle - 500});
            painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), text,
                bodyRect.Moved(new Vector2(2, 2)), Alignment.Center,
                new DrawSettings {Depth = Depth.Middle - 500 + 1, Color = Color.Black});
        }

        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        if (_game.World.PlayerStatistics.SpawnedBoss)
        {
            _tween.Update(dt);
        }
    }
}
