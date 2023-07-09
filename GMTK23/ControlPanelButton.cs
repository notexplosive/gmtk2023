﻿using System;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
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
        var offset = Vector2.Zero;
        if (_isHovered)
        {
            offset = new Vector2(0, 2);

            if (_primed)
            {
                offset = new Vector2(0, 5);
            }
        }

        var percent = Math.Clamp(_cooldownTimer / _wave.Cooldown, 0, 1f);
        
        painter.DrawAsRectangle(Client.Assets.GetTexture("gmtk/button"), _rectangle.Moved(offset), new DrawSettings{Depth = Depth.Middle});

        Client.Assets.GetAsset<SpriteSheet>("ButtonTags").DrawFrameAtPosition(painter, _wave.Stats.TagFrame, _rectangle.Moved(offset).Center, Scale2D.One, new DrawSettings{Depth = Depth.Middle-1, Origin = DrawOrigin.Center});
        
        // painter.DrawAsRectangle(Client.Assets.GetTexture("gmtk/button"), _rectangle.Moved(offset), new DrawSettings{Depth = Depth.Middle});
        painter.DrawRectangle(
            RectangleF.FromCorners(_rectangle.TopLeft,
                Vector2Extensions.Lerp(_rectangle.BottomLeft, _rectangle.BottomRight,
                    percent)),
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
