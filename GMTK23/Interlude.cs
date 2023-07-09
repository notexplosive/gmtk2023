using System.Diagnostics;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Interlude : Widget, IEarlyDrawHook
{
    public Interlude(RectangleF rectangle) : base(rectangle, Depth.Middle)
    {
    }

    public void EarlyDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        
        painter.BeginSpriteBatch();
        painter.DrawLine(Vector2.Zero, Canvas.Size.ToVector2(), new LineDrawSettings{Color = Color.White});
        painter.EndSpriteBatch();
        
        Client.Graphics.PopCanvas();
    }

    public void InitializeWith(PlayerStatistics playerStatistics)
    {
        if (playerStatistics.SpawnedBoss)
        {
            Client.Debug.Log("Won");
        }
        else
        {
            Client.Debug.Log("Lost");
        }
    }
}
