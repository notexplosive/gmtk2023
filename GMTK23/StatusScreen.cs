using System;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class StatusScreen : Widget, IEarlyDrawHook
{
    private readonly Game _game;

    public StatusScreen(RectangleF windowRectangle, Game game) : base(windowRectangle, ExplogineCore.Data.Depth.Middle)
    {
        _game = game;
    }

    public void EarlyDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch();

        var bodyRect = ContentRectangle.MovedToZero();
        
        painter.DrawRectangle(bodyRect, new DrawSettings{Depth = ExplogineCore.Data.Depth.Middle + 50, Color = Color.White.DimmedBy(0.9f)});
        float percent = _game.World.PlayerStatistics.BossMeter;
        painter.DrawRectangle(RectangleF.FromCorners(bodyRect.BottomLeft, Vector2Extensions.Lerp(bodyRect.BottomRight, bodyRect.TopRight, percent)), new DrawSettings{Depth = ExplogineCore.Data.Depth.Middle, Color = Color.Orange});
        
        painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "BOSS METER", bodyRect, Alignment.Center, new DrawSettings{Depth = ExplogineCore.Data.Depth.Middle - 500});
        painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "BOSS METER", bodyRect.Moved(new Vector2(2,2)), Alignment.Center, new DrawSettings{Depth = ExplogineCore.Data.Depth.Middle - 500 + 1, Color = Color.Black});
        
        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }
}
