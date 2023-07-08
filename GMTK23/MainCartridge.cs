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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GMTK23;

public class MainCartridge : BasicGameCartridge
{
    private readonly GameLayout _layout;
    private Rail _rail;

    public MainCartridge(IRuntime runtime) : base(runtime)
    {
        _layout = new GameLayout();
    }

    public override CartridgeConfig CartridgeConfig { get; } =
        new(new Point(768, 432), SamplerState.PointWrap);

    public override void OnCartridgeStarted()
    {
        _layout.Compute(CartridgeConfig.RenderResolution!.Value);

        _rail = new Rail();
        var game = new Game(_layout.Game);
        _rail.Add(game);
        _rail.Add(game.World);
        _rail.Add(game.World.Entities);
        
        var controlPanel = new ControlPanel(_layout.Controls, ScriptContent.Summons(game).ToList());
        _rail.Add(controlPanel);

        var playerPane = new PlayerPane(_layout.Player, game);
        _rail.Add(playerPane);
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
    }

    public override void Draw(Painter painter)
    {
        _rail.EarlyDraw(painter);

        painter.BeginSpriteBatch();

        painter.DrawRectangle(_layout.Feedback, new DrawSettings());
        // painter.DrawRectangle(_layout.Player, new DrawSettings());
        // painter.DrawRectangle(_layout.Game, new DrawSettings());
        // painter.DrawRectangle(_layout.Controls, new DrawSettings());

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
                var tileCountSize = 10;
                var tileSize = 16;
                var canvas = new Canvas(tileSize * tileCountSize, tileSize * tileCountSize);

                Client.Graphics.PushCanvas(canvas);
                painter.BeginSpriteBatch();

                var weightedRandom = new int[]
                {
                    // rocks
                    0,
                    1,
                    2,
                    
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