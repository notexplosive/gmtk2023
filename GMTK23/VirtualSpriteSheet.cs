using System;
using System.Collections.Generic;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMTK23;

public class VirtualSpriteSheet : SpriteSheet
{
    private readonly List<Rectangle> _frameRects = new();

    public VirtualSpriteSheet(Texture2D texture) : base(texture)
    {
    }

    public override int FrameCount => _frameRects.Count;

    public override void DrawFrameAtPosition(Painter painter, int index, Vector2 position, Scale2D scale,
        DrawSettings drawSettings)
    {
        var frameSize = _frameRects[index].Size;
        var isValid = index >= 0 && index <= FrameCount - 1;
        if (!isValid)
        {
            throw new IndexOutOfRangeException();
        }

        var adjustedFrameSize = frameSize.ToVector2() * scale.Value;
        var destinationRect = new RectangleF(position, adjustedFrameSize);

        DrawFrameAsRectangle(painter, index, destinationRect, drawSettings);
    }

    public override Rectangle GetSourceRectForFrame(int index)
    {
        return _frameRects[index];
    }

    public void AddFrame(Rectangle frameRect)
    {
        _frameRects.Add(frameRect);
    }
}
