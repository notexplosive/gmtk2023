using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GMTK23;

public class Game : IEarlyDrawHook, IDrawHook, IUpdateHook, IUpdateInputHook
{
    private readonly List<BackgroundObject> _backgroundSprites;
    private readonly Camera _camera;
    private readonly Canvas _canvas;
    private readonly HoverState _hoverState = new();
    private PlayerShip _player;

    public World World = null!;
    private Vector2 _mousePos;
    private RectangleF _scrollingCamera;
    private readonly MainCartridge _mainCartridge;
    private RectangleF _windowRect;
    private Rail _rail;

    public Game(MainCartridge mainCartridge,RectangleF windowRect)
    {
        _mainCartridge = mainCartridge;
        _windowRect = windowRect;
        var renderResolution = windowRect.Size.ToPoint();
        _canvas = new Canvas(renderResolution);

        var cameraRect = new RectangleF(Vector2.Zero, renderResolution.ToVector2());
        _camera = new Camera(cameraRect, renderResolution);
        _scrollingCamera = cameraRect;

        _backgroundSprites = new List<BackgroundObject>();

        var backgroundSpriteCount = 3;
        for (var i = 0; i < backgroundSpriteCount + 1; i++)
        {
            var x = 420 / backgroundSpriteCount * i - 32;
            _backgroundSprites.Add(new BackgroundObject(
                Client.Assets.GetAsset<SpriteSheet>("BigSheet"),
                new Vector2(
                    x,
                    x + 128 * Client.Random.Dirty.NextFloat()
                )
            ));
        }
        
        Reboot();
    }
    
    public void Reboot()
    {
        _rail = new Rail();
        Global.MusicPlayer.Play();
        World = new World(_windowRect.Size);
        _player = new PlayerShip(new PlayerPersonality());
        World.Entities.AddImmediate(_player);
        _player.Position = new Vector2(_windowRect.Size.X / 2, -100);

        var playerPositionTweenable = new TweenableVector2(() => _player.Position, val => _player.Position = val);

        var startupSequence = new SequenceTween();
        startupSequence
            .Add(new WaitSecondsTween(3f))
            .Add(new CallbackTween(() =>
            {
                World.QuarterInserted = true; 
                Global.PlaySound("gmtk23_jingle1");
            }))
            .Add(playerPositionTweenable.TweenTo(new Vector2(_windowRect.Size.X / 2, 50), 1f, Ease.CubicFastSlow))
            .Add(new CallbackTween(() => { World.IsStarted = true; }))
            ;

        World.ActiveTween.AddChannel(
            startupSequence
            );
        
        _rail.Add(World);
        _rail.Add(World.Entities);
        
        World.OnGameOver += ()=>
        {
            _mainCartridge.SwitchToInterlude();
            // _mainCartridge.SwitchToGameplay();
        };
    }


    public Vector2 Position
    {
        get => _windowRect.Location;
        set => _windowRect.Location = value;
    }

    public void Draw(Painter painter)
    {
        _rail.Draw(painter);
        painter.DrawRectangle(_windowRect, new DrawSettings {Color = Color.Red});
        painter.DrawAsRectangle(_canvas.Texture, _windowRect, new DrawSettings());
    }

    public void EarlyDraw(Painter painter)
    {
        _rail.EarlyDraw(painter);
        Client.Graphics.PushCanvas(_canvas);
        // draw background
        painter.BeginSpriteBatch(_camera.CanvasToScreen);
        var backgroundTile = Client.Assets.GetTexture("BackgroundTexture");

        painter.DrawAsRectangle(backgroundTile, _canvas.Size.ToRectangleF(),
            new DrawSettings
                {SourceRectangle = new Rectangle(_scrollingCamera.Location.ToPoint(), _canvas.Size)});

        painter.EndSpriteBatch();

        // draw background objects

        painter.BeginSpriteBatch(_camera.CanvasToScreen);

        foreach (var sprite in _backgroundSprites)
        {
            sprite.Draw(painter);
        }

        painter.EndSpriteBatch();

        // draw entities
        painter.BeginSpriteBatch(_camera.CanvasToScreen);

        foreach (var entity in World.Entities)
        {
            if (entity is Vfx)
            {
                continue;
            }

            entity.Draw(painter);
        }

        painter.EndSpriteBatch();

        // draw effects
        painter.BeginSpriteBatch(_camera.CanvasToScreen);

        foreach (var entity in World.Entities)
        {
            if (entity is Vfx)
            {
                entity.Draw(painter);
            }
        }

        painter.EndSpriteBatch();

        // draw overlay
        painter.BeginSpriteBatch(_camera.CanvasToScreen);

        World.DrawOverlay(painter);

        painter.EndSpriteBatch();

        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        _rail.Update(dt);

        var bgSpeed = dt * 30;
        _scrollingCamera.Location += new Vector2(0, bgSpeed);

        foreach (var sprite in _backgroundSprites)
        {
            sprite.MoveUpBy(bgSpeed);
        }

        // var rect = new RectangleF(_mousePos, Vector2.Zero).Inflated(30, 30);
        //
        // var foundCells = new HashSet<HeatmapCell>();
        // foreach (var cell in _player.Heatmap.GetCellsWithin(rect))
        // {
        //     if (foundCells.Contains(cell))
        //     {
        //         Client.Debug.Log("dupe");
        //     }
        //         
        //     foundCells.Add(cell);
        //     cell.Want += dt * 5;
        // }
    }

    public void UpdateInput(ConsumableInput input, HitTestStack parentHitTestStack)
    {
        _rail.UpdateInput(input,parentHitTestStack);

        if (!Client.Debug.IsPassiveOrActive)
        {
            return;
        }

        if (input.Keyboard.GetButton(Keys.Q).WasPressed)
        {
            _player.Destroy();
        }

        var hitTestStack = parentHitTestStack.AddLayer(
            _windowRect.CanvasToScreen(_windowRect.Size.ToPoint()) * _camera.ScreenToCanvas, Depth.Middle, _windowRect);

        hitTestStack.AddInfiniteZone(Depth.Middle, _hoverState);
        _mousePos = input.Mouse.Position(hitTestStack.WorldMatrix);
        if (_hoverState)
        {
            if (input.Mouse.GetButton(MouseButton.Left).WasPressed)
            {
                World.Entities.DeferredActions.Add(() =>
                {
                    // var ent = World.Entities.AddImmediate(new EnemyShip(new ShipStats(1,3, new Vector2(10,10))));
                    // ent.Position = _mousePos;
                });
            }

            if (input.Mouse.GetButton(MouseButton.Right).WasPressed)
            {
                _player.TargetPosition = _mousePos;
            }
        }
    }
}
