using System;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Vfx : Entity
{
    private readonly SpriteSheet _spriteSheet;
    private readonly Vector2 _position;
    private readonly float _speed;
    private float _frameTime;

    public Vfx(SpriteSheet spriteSheet, Vector2 position, float speed)
    {
        _spriteSheet = spriteSheet;
        _position = position;
        _speed = speed;
        _frameTime = 0;
    }

    public override void Draw(Painter painter)
    {
        _spriteSheet.DrawFrameAtPosition(painter, Frame(), _position, Scale2D.One, new DrawSettings{Origin = DrawOrigin.Center});
    }

    public override void Update(float dt)
    {
        _frameTime += dt * 60 * _speed;

        if (_frameTime > _spriteSheet.FrameCount)
        {
            Destroy();
        }
    }

    public int Frame()
    {
        return Math.Min((int)_frameTime, _spriteSheet.FrameCount-1);
    }
}
