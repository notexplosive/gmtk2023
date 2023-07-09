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
        painter.DrawRectangle(bodyRect, new DrawSettings());
        
        painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 16),"Focused", bodyRect, Alignment.TopCenter, new DrawSettings{Color = Color.Black});
        var barRect = bodyRect.Inflated(-10, -10).Moved(new Vector2(0, 10));
        
        painter.DrawLineRectangle(barRect, new LineDrawSettings{Color = Color.Black, Depth = Depth.Front});
        var halfWidth = barRect.Width / 2f;

        var currentX = barRect.Center.X + halfWidth * _game.World.PlayerStatistics.IntensityAsBidirectionalPercent;

        painter.DrawLine(new Vector2(currentX, barRect.Top), new Vector2(currentX, barRect.Bottom),new LineDrawSettings{Color = Color.Black, Depth = Depth.Front});
        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        _time += dt;
    }
}
