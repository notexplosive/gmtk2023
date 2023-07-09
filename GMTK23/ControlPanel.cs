using System;
using System.Collections.Generic;
using System.Linq;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class ControlPanel : Widget, IUpdateInputHook, IEarlyDrawHook, IUpdateHook
{
    private readonly RectangleF _windowRectangle;
    private readonly Game _game;
    private readonly List<ControlPanelButton> _buttons;
    private float _idleTimer;
    private bool _hasDoneSomething;

    public ControlPanel(RectangleF windowRectangle, Game game, List<Wave> summons) : base(windowRectangle, ExplogineCore.Data.Depth.Middle)
    {
        _windowRectangle = windowRectangle;
        _game = game;
        _buttons = new List<ControlPanelButton>();

        var size = new Vector2(79, 43);
        var rect = new RectangleF(Vector2.Zero, size);

        foreach (var summon in summons)
        {
            var button = new ControlPanelButton(rect, summon);
            _buttons.Add(button);
            rect = NextRect(rect, size, 4, _windowRectangle.MovedToZero());

            button.WasPressed += () => _hasDoneSomething = true;

        }
    }

    private RectangleF NextRect(RectangleF current, Vector2 size, float padding, RectangleF container)
    {
        var next = current.Moved(size.JustX() + new Vector2(padding,0));
        
        if (!container.Contains(next))
        {
            next = new RectangleF(new Vector2(container.X, current.Y + size.Y + padding), size);
        }
        
        return next;
    }

    public void UpdateInput(ConsumableInput input, HitTestStack parentHitTestStack)
    {
        if (!_game.World.IsStarted)
        {
            return;
        }
        
        UpdateHovered(parentHitTestStack);
        
        var hitTestStack = parentHitTestStack.AddLayer(
            OutputRectangle.CanvasToScreen(ContentRectangle.Size.ToPoint()), Depth, OutputRectangle);

        foreach (var button in _buttons)
        {
            button.UpdateInput(input, hitTestStack);
        }
    }

    public void EarlyDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch();
        foreach (var button in _buttons)
        {
            button.Draw(painter);
        }
        painter.EndSpriteBatch();
        
        painter.BeginSpriteBatch();
        if (!_game.World.IsStarted)
        {
            painter.DrawRectangle(new RectangleF(Vector2.Zero, Size.ToVector2()),
                new DrawSettings {Color = Color.Black.WithMultipliedOpacity(0.5f)});
        }

        if (_idleTimer > 3 && !_hasDoneSomething)
        {
            var inflate = -Math.Abs(MathF.Sin(_idleTimer * 5f) * 5);
            var rect = _buttons[0].Rectangle.Inflated(inflate, inflate);
            painter.DrawLineRectangle(rect, new LineDrawSettings{Color = Color.Cyan, Thickness = 1});
            painter.DrawStringWithinRectangle(Global.GetFont(20), "Click!", rect, Alignment.Center, new DrawSettings{Color = Color.Cyan, Depth= ExplogineCore.Data.Depth.Middle});
            painter.DrawStringWithinRectangle(Global.GetFont(20), "Click!", rect.Moved(new Vector2(1)), Alignment.Center, new DrawSettings{Color = Color.Black, Depth = ExplogineCore.Data.Depth.Middle + 1});
        }
        
        painter.EndSpriteBatch();
        
        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        if (_game.World.IsStarted)
        {
            _idleTimer += dt;
        }

        foreach (var button in _buttons)
        {
            button.Update(dt);
        }
    }
}