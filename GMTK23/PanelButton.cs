using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class PanelButton : IUpdateInputHook, IDrawHook
{
    private readonly HoverState _isHovered = new();
    private readonly RectangleF _rectangle;
    private readonly Summon _summon;
    private bool _primed;

    public PanelButton(RectangleF rectangle, Summon summon)
    {
        _rectangle = rectangle;
        _summon = summon;
    }

    public void Draw(Painter painter)
    {
        var color = Color.Blue;
        if (_isHovered)
        {
            color = Color.LightBlue;
        }

        painter.DrawRectangle(_rectangle, new DrawSettings {Color = color});
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        hitTestStack.AddZone(_rectangle, Depth.Middle, _isHovered);

        if (input.Mouse.GetButton(MouseButton.Left).WasReleased)
        {
            if (_primed && _isHovered)
            {
                _summon.Execute();
            }
            
            _primed = false;
        }
        
        if (_isHovered && input.Mouse.GetButton(MouseButton.Left).WasPressed)
        {
            _primed = true;
        }
    }
}

public class Summon
{
    private readonly Game _game;
    private List<ShipSpawn> _shipSpawns = new();

    public Summon(Game game)
    {
        _game = game;
    }

    public void Execute()
    {
        foreach (var spawn in _shipSpawns)
        {
            var enemyShip = _game.World.Entities.AddImmediate(new EnemyShip(spawn.Stats));
            enemyShip.Position = spawn.Position;
        }
    }

    public void SpawnShipAt(ShipStats stats, Vector2 vector2)
    {
        _shipSpawns.Add(new ShipSpawn(stats,vector2));
    }
}

public record ShipSpawn(ShipStats Stats, Vector2 Position);
