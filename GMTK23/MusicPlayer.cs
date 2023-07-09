using System;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExTween;
using Microsoft.Xna.Framework.Audio;

namespace GMTK23;

public class MusicPlayer
{
    private readonly TweenableFloat _intrTrackVolumeTweenable = new();
    private readonly TweenableFloat _bossTrackVolumeTweenable = new();
    private readonly TweenableFloat _mainTrackVolumeTweenable = new();
    private readonly SequenceTween _tween = new();
    private SoundEffectInstance _intrTrack = null!;
    private SoundEffectInstance _bossTrack = null!;
    private SoundEffectInstance _mainTrack = null!;

    /// <summary>
    ///     needs content to be ready, cannot be constructor
    /// </summary>
    public void Initialize()
    {
        _mainTrack = Client.SoundPlayer.Play("gmtk/music_main", new SoundEffectSettings {Loop = true, Volume = 0});
        _intrTrack = Client.SoundPlayer.Play("gmtk/music_interlude", new SoundEffectSettings {Loop = true, Volume = 0});
        _bossTrack = Client.SoundPlayer.Play("gmtk/music_boss", new SoundEffectSettings {Loop = true, Volume = 0});
    }

    public void Play()
    {
        _mainTrack.Play();
        _intrTrack.Play();
        _bossTrack.Play();

        _mainTrackVolumeTweenable.Value = 1;
        _intrTrackVolumeTweenable.Value = 0;
        _bossTrackVolumeTweenable.Value = 0;
    }

    public void Stop()
    {
        _mainTrack.Stop();
        _bossTrack.Stop();
        _intrTrack.Stop();
    }

    public void FadeToMain()
    {
        var fadeDuration = 0.25f;
        _tween.Add(new MultiplexTween()
                .AddChannel(_mainTrackVolumeTweenable.TweenTo(1, fadeDuration, Ease.Linear))
                .AddChannel(_intrTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear))
                .AddChannel(_bossTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear))
            );
    }

    public void FadeToInterlude()
    {
        var fadeDuration = 0.25f;
        _tween.Add(new MultiplexTween()
            .AddChannel(_mainTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear))
            .AddChannel(_intrTrackVolumeTweenable.TweenTo(1, fadeDuration, Ease.Linear))
            .AddChannel(_bossTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear))
        );
    }
    
    public void FadeToBoss(Action callback)
    {
        var fadeDuration = 0.25f;
        _tween.Add(new MultiplexTween()
            .AddChannel(_mainTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear))
            .AddChannel(_intrTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear))
            .AddChannel(_bossTrackVolumeTweenable.TweenTo(0, fadeDuration, Ease.Linear))
        );
        _tween.Add(new CallbackTween(() => Global.PlaySound("gmtk23_levelcomplete")));
        // wait for jingle to be over
        _tween.Add(new WaitSecondsTween(4));
        _tween.Add(new CallbackTween(() => Global.PlaySound("gmtk23_enemy8", 1f)));
        // pause for (evil) laugh
        _tween.Add(new WaitSecondsTween(1.5f));
        _tween.Add(
            _bossTrackVolumeTweenable.TweenTo(1, fadeDuration, Ease.Linear)
        );
        _tween.Add(new CallbackTween(callback));
    }

    public void UpdateFader(float dt)
    {
        _mainTrack.Volume = _mainTrackVolumeTweenable.Value * 0.35f;
        _intrTrack.Volume = _intrTrackVolumeTweenable.Value * 0.8f;
        _bossTrack.Volume = _bossTrackVolumeTweenable.Value * 0.35f;

        _tween.Update(dt);
        if (_tween.IsDone())
        {
            _tween.Clear();
        }
    }
}
