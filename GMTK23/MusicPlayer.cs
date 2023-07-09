using ExplogineCore.Data;
using ExplogineMonoGame;
using ExTween;
using Microsoft.Xna.Framework.Audio;

namespace GMTK23;

public class MusicPlayer
{
    private MultiplexTween _tween = new();
    private SoundEffectInstance _mainTrack = null!;
    private SoundEffectInstance _intrTrack = null!;
    private readonly TweenableFloat _mainTrackVolumeTweenable = new();
    private readonly TweenableFloat _intrTrackVolumeTweenable = new();

    /// <summary>
    ///  needs content to be ready, cannot be constructor
    /// </summary>
    public void Initialize()
    {
        _mainTrack = Client.SoundPlayer.Play("gmtk/music_main", new SoundEffectSettings {Loop = true, Volume = 0});
        _intrTrack = Client.SoundPlayer.Play("gmtk/music_interlude", new SoundEffectSettings {Loop = true, Volume = 0});
    }
    
    public void Play()
    {
        Stop();
        
        _mainTrack.Play();
        _intrTrack.Play();

        _mainTrackVolumeTweenable.Value = 1;
        _intrTrackVolumeTweenable.Value = 0;
    }

    public void Stop()
    {
        _mainTrack.Stop();
        _intrTrack.Stop();
    }

    public void FadeToMain()
    {
        var fadeDuration = 1;
        _tween.AddChannel(_mainTrackVolumeTweenable.TweenTo(1, fadeDuration, Ease.Linear));
        _tween.AddChannel(_intrTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear));
    }
    
    public void FadeToInterlude()
    {
        var fadeDuration = 1;
        _tween.AddChannel(_mainTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear));
        _tween.AddChannel(_intrTrackVolumeTweenable.TweenTo(1, fadeDuration, Ease.Linear));
    }

    public void UpdateFader(float dt)
    {
        var factor = 0.35f;
        _mainTrack.Volume = _mainTrackVolumeTweenable.Value * 0.35f;
        _intrTrack.Volume = _intrTrackVolumeTweenable.Value * 0.8f;

        _tween.Update(dt);
        if (_tween.IsDone())
        {
            _tween.Clear();
        }
    }
}
