﻿using ExplogineDesktop;
using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;
using GMTK23;
using Microsoft.Xna.Framework;

var config = new WindowConfigWritable
{
    WindowSize = new Point(1600, 900),
    Title = "Pest Control - GMTK 2023"
};
Bootstrap.Run(args, new WindowConfig(config), runtime =>
    {
        Global.MultiCartridge = new MultiCartridge(runtime, new MainCartridge(runtime));
        return Global.MultiCartridge;
    }
#if !DEBUG
    ,
    "--fullscreen"
#endif
);
