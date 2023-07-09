using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GMTK23;

public class MainCartridge : BasicGameCartridge
{
    private readonly GameLayout _layout;
    private Rail _rail = new();
    private Game _game = null!;
    private ControlPanel _controlPanel = null!;
    private PlayerPane _playerPane = null!;
    private SequenceTween _tween = new ();
    private StatusScreen _statusScreen;

    public MainCartridge(IRuntime runtime) : base(runtime)
    {
        _layout = new GameLayout();
    }

    public override CartridgeConfig CartridgeConfig { get; } =
        new(new Point(768, 432), SamplerState.PointWrap);

    public override void OnCartridgeStarted()
    {
        _layout.ComputeGameplay(CartridgeConfig.RenderResolution!.Value);

        _rail = new Rail();
        _game = new Game(this,_layout.Game);
        
        _rail.Add(_game);

        _controlPanel = new ControlPanel(_layout.Controls, _game, ScriptContent.Summons(_game).ToList());
        _rail.Add(_controlPanel);

        _playerPane = new PlayerPane(_layout.Player, _game);
        _rail.Add(_playerPane);

        _statusScreen = new StatusScreen(_layout.Status, _game);
        _rail.Add(_statusScreen);
    }

    public void SwitchToInterlude()
    {
        var duration = 1f;
        _layout.ComputeInterlude(CartridgeConfig.RenderResolution!.Value);

        var gameTweenable = new TweenableVector2(() => _game.Position, val => _game.Position = val);
        var controlPanelTweenable = new TweenableVector2(()=> _controlPanel.Position, (val)=> _controlPanel.Position = val);
        var playerPaneTweenable = new TweenableVector2(()=> _playerPane.Position, (val)=> _playerPane.Position = val);
        var statusTweenable = new TweenableVector2(()=> _statusScreen.Position, (val)=> _statusScreen.Position = val);


        _tween.Add(new WaitSecondsTween(1));
        _tween.Add(new MultiplexTween()
            .AddChannel(playerPaneTweenable.TweenTo(_layout.Player.Location, 1f, Ease.QuadFastSlow))
            .AddChannel(controlPanelTweenable.TweenTo(_layout.Controls.Location, 1f, Ease.QuadFastSlow))
            .AddChannel(gameTweenable.TweenTo(_layout.Game.Location, 1f, Ease.QuadFastSlow))
            .AddChannel(statusTweenable.TweenTo(_layout.Status.Location, 1f, Ease.QuadFastSlow))
        );
    }

    public void SwitchToGameplay()
    {
        var duration = 1f;
        _layout.ComputeGameplay(CartridgeConfig.RenderResolution!.Value);

        var gameTweenable = new TweenableVector2(() => _game.Position, val => _game.Position = val);
        var controlPanelTweenable = new TweenableVector2(()=> _controlPanel.Position, (val)=> _controlPanel.Position = val); 
        var playerPaneTweenable = new TweenableVector2(()=> _playerPane.Position, (val)=> _playerPane.Position = val);
        var statusTweenable = new TweenableVector2(()=> _statusScreen.Position, (val)=> _statusScreen.Position = val);

        _tween.Add(new CallbackTween(() => _game.Reboot()));
        _tween.Add(new WaitSecondsTween(1));
        _tween.Add(new MultiplexTween()
            .AddChannel(playerPaneTweenable.TweenTo(_layout.Player.Location, 1f, Ease.QuadFastSlow))
            .AddChannel(controlPanelTweenable.TweenTo(_layout.Controls.Location, 1f, Ease.QuadFastSlow))
            .AddChannel(gameTweenable.TweenTo(_layout.Game.Location, 1f, Ease.QuadFastSlow))
            .AddChannel(statusTweenable.TweenTo(_layout.Status.Location, 1f, Ease.QuadFastSlow))
        );
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _rail.UpdateInput(input, hitTestStack);

        if (Client.Debug.IsPassiveOrActive)
        {
            if (input.Keyboard.GetButton(Keys.R).WasPressed && input.Keyboard.Modifiers.Control)
            {
                Global.MultiCartridge.RegenerateCurrentCartridge();
            }
        }
    }

    public override void Update(float dt)
    {
        _rail.Update(dt);
        _tween.Update(dt);

        if (_tween.IsDone())
        {
            _tween.Clear();
        }
    }

    public override void Draw(Painter painter)
    {
        _rail.EarlyDraw(painter);

        painter.BeginSpriteBatch();
        _rail.Draw(painter);
        painter.EndSpriteBatch();
    }

    public override void AddCommandLineParameters(CommandLineParametersWriter parameters)
    {
    }

    public override IEnumerable<ILoadEvent> LoadEvents(Painter painter)
    {
        yield return new AssetLoadEvent("Ships",
            () => new GridBasedSpriteSheet(Client.Assets.GetTexture("gmtk/sheet"), new Point(32, 32)));
        
        yield return new AssetLoadEvent("white-ships-sheet",
            () =>
            {
                var sheet = Client.Assets.GetTexture("gmtk/sheet");
                var canvas = new Canvas(sheet.Width, sheet.Height);
                
                Client.Graphics.PushCanvas(canvas);
                painter.BeginSpriteBatch(Matrix.Identity, Client.Assets.GetEffect("gmtk/Whiten"));
                painter.DrawAtPosition(sheet, Vector2.Zero);
                painter.EndSpriteBatch();
                Client.Graphics.PopCanvas();
                
                return canvas.AsTextureAsset();
            });
        
        yield return new AssetLoadEvent("WhiteShips",
            () => new GridBasedSpriteSheet(Client.Assets.GetTexture("white-ships-sheet"), new Point(32, 32)));
        
        yield return new AssetLoadEvent("Player",
            () => new GridBasedSpriteSheet(Client.Assets.GetTexture("gmtk/player"), new Point(32, 32)));
        
        yield return new AssetLoadEvent("BackgroundTiles",
            () => new GridBasedSpriteSheet(Client.Assets.GetTexture("gmtk/background-tiles"), new Point(16)));
        
        yield return new AssetLoadEvent("BigSheet",
            () =>
            {
                var sheet = new VirtualSpriteSheet(Client.Assets.GetTexture("gmtk/big-sheet"));
                
                sheet.AddFrame(new Rectangle(0,0,64,64));
                sheet.AddFrame(new Rectangle(64,0,64,64));
                sheet.AddFrame(new Rectangle(192,0,64,64));
                return sheet;
            });

        yield return new AssetLoadEvent("Explosion",
            () => new GridBasedSpriteSheet(Client.Assets.GetTexture("gmtk/explosion"), new Point(32)));
        
        yield return new AssetLoadEvent("PlayerPane",
            () => new GridBasedSpriteSheet(Client.Assets.GetTexture("gmtk/player-pane"), new Point(162, 208)));
        
        yield return new AssetLoadEvent("BackgroundTexture",
            () =>
            {
                var tileCountSize = 27;
                var tileSize = 16;
                var canvas = new Canvas(tileSize * tileCountSize, tileSize * tileCountSize);

                Client.Graphics.PushCanvas(canvas);
                painter.BeginSpriteBatch();

                var weightedRandom = new int[]
                {

                    // grass
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    3,
                    3,
                    4,
                    4,
                    5,
                    5,
                    
                    // rocks
                    0,
                    1,
                    2,
                };

                var sheet = Client.Assets.GetAsset<SpriteSheet>("BackgroundTiles");
                for (int i = 0; i < tileCountSize; i++)
                {
                    for (int j = 0; j < tileCountSize; j++)
                    {
                        var frame = Client.Random.Dirty.GetRandomElement(weightedRandom);
                        var flip = new XyBool(Client.Random.Dirty.NextBool(), false);
                        sheet.DrawFrameAtPosition(painter, frame, new Vector2(i,j) * tileSize, Scale2D.One, new DrawSettings(){Flip = flip});
                    }
                }
                
                painter.EndSpriteBatch();
                Client.Graphics.PopCanvas();
                
                return canvas.AsTextureAsset();
            });
    }
}