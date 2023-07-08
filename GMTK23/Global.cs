using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;

namespace GMTK23;

public static class Global
{
    public static MultiCartridge MultiCartridge = null!;

    public static SpriteSheet MainSheet => Client.Assets.GetAsset<SpriteSheet>("Ships");
    public static SpriteSheet MainSheetWithFlash => Client.Assets.GetAsset<SpriteSheet>("WhiteShips");
    public static SpriteSheet PlayerSheet => Client.Assets.GetAsset<SpriteSheet>("Player");
}
