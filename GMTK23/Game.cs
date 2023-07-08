using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Game : IEarlyDrawHook, IDrawHook, IUpdateHook, IUpdateInputHook
{
    private readonly Camera _camera;
    private readonly Canvas _canvas;
    private readonly HoverState _hoverState = new();
    private readonly Camera _scrollingCamera;
    private readonly RectangleF _windowRect;

    public readonly World World;
    private readonly PlayerShip _player;

    public Game(RectangleF windowRect)
    {
        _windowRect = windowRect;
        var renderResolution = windowRect.Size.ToPoint();
        _canvas = new Canvas(renderResolution);

        var cameraRect = new RectangleF(Vector2.Zero, renderResolution.ToVector2());
        _camera = new Camera(cameraRect, renderResolution);
        _scrollingCamera = new Camera(cameraRect, renderResolution);

        World = new World(_windowRect.Size);
        _player = new PlayerShip(new PlayerPersonality()); 
        World.Entities.AddImmediate(_player);
        _player.Position = new Vector2(windowRect.Size.X / 2, 50);
    }

    public void Draw(Painter painter)
    {
        painter.DrawRectangle(_windowRect, new DrawSettings {Color = Color.Red});
        painter.DrawAsRectangle(_canvas.Texture, _windowRect, new DrawSettings());
    }

    public void EarlyDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(_canvas);
        // draw background
        painter.BeginSpriteBatch(_camera.CanvasToScreen);
        var backgroundTile = Client.Assets.GetTexture("gmtk/tiles");

        painter.DrawAsRectangle(backgroundTile, _canvas.Size.ToRectangleF(),
            new DrawSettings
                {SourceRectangle = new Rectangle(_scrollingCamera.TopLeftPosition.ToPoint(), _canvas.Size)});

        painter.EndSpriteBatch();

        // draw entities
        painter.BeginSpriteBatch(_camera.CanvasToScreen);

        foreach (var entity in World.Entities)
        {
            entity.Draw(painter);
        }

        painter.EndSpriteBatch();

        // draw effects
        painter.BeginSpriteBatch(_camera.CanvasToScreen);

        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        _scrollingCamera.CenterPosition += new Vector2(0, dt * 60);
    }

    public void UpdateInput(ConsumableInput input, HitTestStack parentHitTestStack)
    {
        if (!Client.Debug.IsPassiveOrActive)
        {
            return;
        }

        var hitTestStack = parentHitTestStack.AddLayer(
            _windowRect.CanvasToScreen(_windowRect.Size.ToPoint()) * _camera.ScreenToCanvas, Depth.Middle, _windowRect);

        hitTestStack.AddInfiniteZone(Depth.Middle, _hoverState);
        var mousePos = input.Mouse.Position(hitTestStack.WorldMatrix);
        if (_hoverState)
        {
            if (input.Mouse.GetButton(MouseButton.Left).WasPressed)
            {
                World.Entities.DeferredActions.Add(() =>
                {
                    var ent = World.Entities.AddImmediate(new EnemyShip(1));
                    ent.Position = mousePos;
                });
            }

            if (input.Mouse.GetButton(MouseButton.Right).WasPressed)
            {
                _player.TargetPosition = mousePos;
            }
        }
    }
}
