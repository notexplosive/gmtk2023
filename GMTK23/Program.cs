using ExplogineDesktop;
using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;
using GMTK23;
using Microsoft.Xna.Framework;

var config = new WindowConfigWritable
{
    WindowSize = new Point(1600, 900),
    Title = "NotExplosive.net",
    #if !DEBUG
    Fullscreen = true,
    #endif
};
Bootstrap.Run(args, new WindowConfig(config), (runtime) => new MainCartridge(runtime));