using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GMTK23;

public static class Global
{
    public static MultiCartridge MultiCartridge = null!;

    public static SpriteSheet ShipsSheet => Client.Assets.GetAsset<SpriteSheet>("Ships");
}

public class MainCartridge : BasicGameCartridge
{
    private readonly GameLayout _layout;
    private Rail _rail;

    public MainCartridge(IRuntime runtime) : base(runtime)
    {
        _layout = new GameLayout();

        for (int i = 1; i < 100; i++)
        {
            var possibleResolution = new Point(16, 9).Multiplied(i);
            Client.Debug.Log(possibleResolution);
        }
    }

    public override CartridgeConfig CartridgeConfig { get; } =
        new(new Point(1280, 720), SamplerState.PointWrap);

    public override void OnCartridgeStarted()
    {
        _layout.Compute(CartridgeConfig.RenderResolution!.Value);

        _rail = new Rail();
        _rail.Add(new Game(_layout.Game));
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
        painter.DrawRectangle(_layout.Player, new DrawSettings());
        painter.DrawRectangle(_layout.Game, new DrawSettings());
        painter.DrawRectangle(_layout.Ui, new DrawSettings());

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
    }
}
