using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;
using Camera = ExplogineMonoGame.Camera;

namespace GMTK23;

public class Game : IEarlyDrawHook, IDrawHook, IUpdateHook
{
    private readonly Camera _camera;
    private readonly Canvas _canvas;
    private readonly RectangleF _windowRect;
    private readonly Camera _scrollingCamera;

    public Game(RectangleF windowRect)
    {
        _windowRect = windowRect;
        var renderResolution = windowRect.Size.ToPoint();
        _canvas = new Canvas(renderResolution);

        var cameraRect = new RectangleF(Vector2.Zero, renderResolution.ToVector2());
        _camera = new Camera(cameraRect, renderResolution);
        _scrollingCamera = new Camera(cameraRect, renderResolution);
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
}
