using ExplogineDesktop;
using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;
using GMTK23;
using Microsoft.Xna.Framework;

var config = new WindowConfigWritable
{
    WindowSize = new Point(1600, 900),
    Title = "GMTK 2023",
#if !DEBUG
    Fullscreen = true,
#endif
};
Bootstrap.Run(args, new WindowConfig(config), runtime =>
{
    Global.MultiCartridge = new MultiCartridge(runtime, new MainCartridge(runtime));
    return Global.MultiCartridge;
});
