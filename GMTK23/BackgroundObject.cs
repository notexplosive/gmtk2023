using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class BackgroundObject
{
    private int _frame;
    private Vector2 _position;
    private readonly SpriteSheet _sheet;

    public BackgroundObject(SpriteSheet sheet, Vector2 position)
    {
        _sheet = sheet;
        _frame = Client.Random.Dirty.NextInt(0, sheet.FrameCount + 1);
        _position = position;
    }

    public RectangleF Rectangle => new(_position, _sheet.GetSourceRectForFrame(0).Size.ToVector2());

    public void Draw(Painter painter)
    {
        _sheet.DrawFrameAtPosition(painter, _frame, _position, Scale2D.One, new DrawSettings());
    }

    public void MoveUpBy(float dt)
    {
        _position -= new Vector2(0,dt);

        if (Rectangle.Bottom < 0)
        {
            _frame = Client.Random.Dirty.NextInt(0, _sheet.FrameCount + 1);
            _position = new Vector2(_position.X, 420 + 64 + Client.Random.Dirty.NextFloat() * 240);
        }
    }
}
