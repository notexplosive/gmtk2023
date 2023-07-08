using System.Collections.Generic;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class ControlPanel : Widget, IUpdateInputHook, IEarlyDrawHook, IUpdateHook
{
    private readonly RectangleF _windowRectangle;
    private readonly List<ControlPanelButton> _buttons;

    public ControlPanel(RectangleF windowRectangle, List<Wave> summons) : base(windowRectangle, ExplogineCore.Data.Depth.Middle)
    {
        _windowRectangle = windowRectangle;
        _buttons = new List<ControlPanelButton>();

        var size = new Vector2(79, 43);
        var rect = new RectangleF(Vector2.Zero, size);

        foreach (var summon in summons)
        {
            _buttons.Add(new ControlPanelButton(rect, summon));
            rect = NextRect(rect, size, 4, _windowRectangle.MovedToZero());
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
        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        foreach (var button in _buttons)
        {
            button.Update(dt);
        }
    }
}