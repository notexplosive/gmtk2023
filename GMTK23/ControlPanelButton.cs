using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class ControlPanelButton : IUpdateInputHook, IDrawHook, IUpdateHook
{
    private readonly HoverState _isHovered = new();
    private readonly RectangleF _rectangle;
    private readonly Wave _wave;
    private float _cooldownTimer;
    private bool _primed;

    public ControlPanelButton(RectangleF rectangle, Wave wave)
    {
        _rectangle = rectangle;
        _wave = wave;
    }

    public void Draw(Painter painter)
    {
        var color = Color.Blue;
        if (_isHovered)
        {
            color = Color.LightBlue;
        }

        painter.DrawRectangle(_rectangle, new DrawSettings {Color = color, Depth = Depth.Middle});
        painter.DrawRectangle(
            RectangleF.FromCorners(_rectangle.TopLeft,
                Vector2Extensions.Lerp(_rectangle.BottomLeft, _rectangle.BottomRight,
                    _cooldownTimer / _wave.Cooldown)),
            new DrawSettings {Color = Color.Black.WithMultipliedOpacity(0.5f), Depth = Depth.Middle - 1000});
    }

    public void Update(float dt)
    {
        _cooldownTimer -= dt;
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (_cooldownTimer > 0)
        {
            _isHovered.Unset();
            return;
        }
        
        hitTestStack.AddZone(_rectangle, Depth.Middle, _isHovered);

        if (input.Mouse.GetButton(MouseButton.Left).WasReleased)
        {
            if (_primed && _isHovered)
            {
                _wave.Execute();
                _cooldownTimer = _wave.Cooldown;
            }

            _primed = false;
        }

        if (_isHovered && input.Mouse.GetButton(MouseButton.Left).WasPressed)
        {
            _primed = true;
        }
        
    }
}

public record ShipSpawn(ShipStats Stats, Vector2 Position);
