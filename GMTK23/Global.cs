using System.Diagnostics;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;

namespace GMTK23;

public static class Global
{
    public static MultiCartridge MultiCartridge = null!;

    public static SpriteSheet MainSheet => Client.Assets.GetAsset<SpriteSheet>("Ships");
    public static SpriteSheet MainSheetWithFlash => Client.Assets.GetAsset<SpriteSheet>("WhiteShips");
    public static SpriteSheet PlayerSheet => Client.Assets.GetAsset<SpriteSheet>("Player");
    
    public static SpriteSheet BigSheet =>  Client.Assets.GetAsset<SpriteSheet>("BigSheet");
    public static SpriteSheet BigSheetWithFlash =>  Client.Assets.GetAsset<SpriteSheet>("BigSheetWithFlash");

    public static MusicPlayer MusicPlayer = new();

    public static void PlaySound(string soundName)
    {
        var instance = Client.Assets.GetSoundEffectInstance($"gmtk/{soundName}");
        instance.Stop();
        instance.Volume = 0.35f;
        instance.Play();
    }

    public static IFontGetter GetFont(int i)
    {
        return Client.Assets.GetFont("gmtk/GameFont", i);
    }
}