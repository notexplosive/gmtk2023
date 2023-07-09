using System;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Boss : Ship
{
    private float _flashTimer;
    private float _shootTimer;
    private SequenceTween _tween = new();

    public Boss() : base(Team.Enemy, int.MaxValue)
    {
        TookDamage += OnTakeDamage;

        _tween.IsLooping = true;
        var positionTweenable = new TweenableVector2(() => Position, value => Position = value);

        var numberOfPoints = 10;
        for (int i = 0; i < numberOfPoints; i++)
        {
            var target = new Vector2(420 / 2f) + Vector2Extensions.Polar(100, (float)i/numberOfPoints * MathF.PI * 2 + MathF.PI / 2) + new Vector2(0,50);
            _tween.Add(positionTweenable.TweenTo(target, 1f, Ease.Linear));
        }
    }

    public override RectangleF TakeDamageBox => BoundingBox;

    private void OnTakeDamage()
    {
        _flashTimer = 0.1f;
    }

    public override void Update(float dt)
    {
        _flashTimer -= dt;
        _shootTimer -= dt;
        _tween.Update(dt);

        if (_shootTimer < 0)
        {
            Shoot(ScriptContent.BasicEnemyBullet, Client.Random.Dirty.NextNormalVector2());
            _shootTimer = 0.5f;
        }
    }

    public override void Draw(Painter painter)
    {
        var sheet = Global.BigSheet;
        if (_flashTimer > 0)
        {
            sheet = Global.BigSheetWithFlash;
        }
        
        sheet.DrawFrameAtPosition(painter, 3, Position, Scale2D.One, new DrawSettings{Origin = DrawOrigin.Center});
    }
    
    public override bool HasInvulnerabilityFrames()
    {
        return false;
    }
}
