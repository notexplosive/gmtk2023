using System.Diagnostics;
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
    public static MusicPlayer MusicPlayer = new();

    public static void PlaySound(string soundName)
    {
        var instance = Client.Assets.GetSoundEffectInstance($"gmtk/{soundName}");
        instance.Stop();
        instance.Volume = 0.5f;
        instance.Play();
    }
}

public class MusicPlayer
{
    public void Play()
    {
        var track = Client.Assets.GetSoundEffectInstance("gmtk/music_main");

        track.IsLooped = true;
        track.Volume = 0.5f;
        track.Stop();
        track.Play();
    }

    public void Stop()
    {
        var track = Client.Assets.GetSoundEffectInstance("gmtk/music_main");

        track.Stop();
    }
}
