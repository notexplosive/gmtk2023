using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class BackgroundObject
{
    private readonly SpriteSheet _sheet;
    private int _frame;
    private Vector2 _position;
    private bool _flipX;

    public BackgroundObject(SpriteSheet sheet, Vector2 position, Depth depth)
    {
        _sheet = sheet;
        _frame = Client.Random.Dirty.NextInt(0, 4);
        _position = position;
        RenderDepth = depth;
        _flipX = Client.Random.Dirty.NextBool();
    }

    public RectangleF Rectangle => new(_position, _sheet.GetSourceRectForFrame(0).Size.ToVector2());

    public Depth RenderDepth { get; set; }

    public void Draw(Painter painter)
    {
        _sheet.DrawFrameAtPosition(painter, _frame, _position, Scale2D.One, new DrawSettings {Depth = RenderDepth, Flip = new XyBool(_flipX, false)});
    }

    public void MoveUpBy(float dt)
    {
        _position -= new Vector2(0, dt);

        if (Rectangle.Bottom < 0)
        {
            _frame = Client.Random.Dirty.NextInt(0, 4);
            _position = new Vector2(_position.X, _position.Y + 420 + 64);
            _flipX = Client.Random.Dirty.NextBool();
        }
    }
}
