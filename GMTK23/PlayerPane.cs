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
    private float _time;

    public PlayerPane(RectangleF windowRectangle, Game game) : base(windowRectangle, Depth.Middle)
    {
        
    }

    public void EarlyDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch();
        var playerPane = Client.Assets.GetAsset<SpriteSheet>("PlayerPane");
            
        playerPane.DrawFrameAtPosition(painter, 0, Vector2.Zero, Scale2D.One, new DrawSettings{Depth = Depth.Back});
        playerPane.DrawFrameAtPosition(painter, 1, new Vector2(0,MathF.Sin(_time * 10) * 2), Scale2D.One, new DrawSettings{Depth = Depth.Middle});
        var foreground = Depth.Middle - 100;
        playerPane.DrawFrameAtPosition(painter, 2, Vector2.Zero, Scale2D.One, new DrawSettings{Depth = foreground});


        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        _time += dt;
    }
}
