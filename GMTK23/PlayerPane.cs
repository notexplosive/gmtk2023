using System;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class PlayerPane : Widget, IEarlyDrawHook, IUpdateHook
{
    private readonly Game _game;
    private float _time;

    public PlayerPane(RectangleF windowRectangle, Game game) : base(windowRectangle, Depth.Middle)
    {
        _game = game;
    }

    public void EarlyDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch();
        var playerPane = Client.Assets.GetAsset<SpriteSheet>("PlayerPane");
        playerPane.DrawFrameAtPosition(painter, 0, Vector2.Zero, Scale2D.One, new DrawSettings{Depth = Depth.Back - 100});
        playerPane.DrawFrameAtPosition(painter, 1, Vector2.Zero + new Vector2(MathF.Sin(_time * 5),0), Scale2D.One, new DrawSettings{Depth = Depth.Back - 101});
        painter.EndSpriteBatch();
        
        painter.BeginSpriteBatch();
        var bodyRect = new RectangleF(0, 162, 162, 46);

        if (_game.World.PlayerStatistics.BossMeter < 1f)
        {
            painter.DrawRectangle(bodyRect, new DrawSettings{Depth = Depth.Back});
            painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 16), GetStringForIntensity(_game.World.PlayerStatistics.IntensityAsBidirectionalPercent), bodyRect,
                Alignment.TopCenter, new DrawSettings {Color = Color.Black});
            var barRect = bodyRect.Inflated(-10, -10).Moved(new Vector2(0, 10));


            var moodColor = Color.White;
            var biPercent = _game.World.PlayerStatistics.IntensityAsBidirectionalPercent;

            if (biPercent < 0)
            {
                moodColor = Color.DarkBlue.BrightenedBy(1 - Math.Abs(biPercent));
            }
            
            if (biPercent > 0)
            {
                moodColor = Color.DarkRed.BrightenedBy(1 - Math.Abs(biPercent));
            }
            
            
            painter.DrawRectangle(barRect, new DrawSettings {Color = moodColor, Depth = Depth.Front +  100});
            painter.DrawLineRectangle(barRect, new LineDrawSettings {Color = Color.Black, Depth = Depth.Front});
            var halfWidth = barRect.Width / 2f;

            var currentX = barRect.Center.X + halfWidth * _game.World.PlayerStatistics.IntensityAsBidirectionalPercent;

            painter.DrawLine(new Vector2(currentX, barRect.Top), new Vector2(currentX, barRect.Bottom),
                new LineDrawSettings {Color = Color.Black, Depth = Depth.Front});
        }

        else
        {
            painter.DrawRectangle(bodyRect, new DrawSettings{Depth = Depth.Back, Color = Color.Black});
            painter.DrawStringWithinRectangle(Global.GetFont(20),"BOSS TIME",bodyRect, Alignment.Center, new DrawSettings{Color = Color.Red, Depth = Depth.Middle});
        }
        
        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    private string GetStringForIntensity(float percent)
    {
        var result = "Flow";
        if (MathF.Abs(percent) < 0.25f)
        {
            result= "Flow";
        }

        if (percent > 0.25f)
        {
            result = "Stressed";
        }
        
        if (percent > 0.75f)
        {
            result = "Anxious";
        }
        
        if (percent < -0.25f)
        {
            result = "Idle";
        }
        
        if (percent < -0.75f)
        {
            result = "Bored";
        }

        return result;
    }

    public void Update(float dt)
    {
        _time += dt;
    }
}
