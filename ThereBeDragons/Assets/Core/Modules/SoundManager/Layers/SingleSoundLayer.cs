using DG.Tweening;
using System.Collections.Generic;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// An Audio Layer that supports playing only a single clip at a time.
  /// </summary>
  /// <remarks>A Single SOund Layer is usually used for things like background music</remarks>
  class SingleSoundLayer : AbstractSoundLayer
  {
    /// <summary>
    /// The amount of time in seconds a clip will fade in and out
    /// </summary>
    private float _crossFadeDuration = 0f;

    /// <summary>
    /// Indicates if the fade queue/transition is in progress
    /// </summary>
    private bool _fadeInProgress = false;

    /// <summary>
    /// The current playing audio source
    /// </summary>
    private IAudioSourceController _currentAudioSource;

    private AudioContainer _nextClip;

    /// <summary>
    /// Initializes the single audio layer with a crossfade duration to be used when transitioning between clips.
    /// </summary>
    /// <param name="crossFadeDuration">The amount of time in seconds a clip will fade in and out</param>
    public SingleSoundLayer(float crossFadeDuration)
      : base()
    {
      _crossFadeDuration = crossFadeDuration;
    }

    /// <summary>
    /// Diposes of the sound layer
    /// </summary>
    public override void Dispose()
    {
      _fadeInProgress = false;

      if (_currentAudioSource != null)
      {
        _currentAudioSource.Dispose();
      }
    }

    /// <summary>
    /// Verifies that the max instances of the sound on the layer hasn't been reached and that the sound object isn;t already being played.
    /// </summary>
    /// <param name="soundObject">Sound object reference containing max instance data</param>
    /// <returns>True if the sound can be played on the layer</returns>
    public override bool WillAcceptSound(SoundObject soundObject)
    {
      if (_currentAudioSource != null && _currentAudioSource.audioSource != null && _currentAudioSource.soundID == soundObject.audioID)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Plays the audio clip. If a sound is already active, it will be faded out as this one fades in.
    /// </summary>
    /// <param name="audioClip">Audio clip controller.</param>
    /// <param name="loop">Whether to loop the sound or not</param>
    public override void PlaySound(IAudioSourceController audioClip, bool loop = false)
    {
      //If the SoundManager failed to load the audio clip a NullAudioSourceController would be provided to avoid runtime errors
      //In this event the sound ID is empty so we exit this function
      if (string.IsNullOrEmpty(audioClip.soundID))
      {
        return;
      }

      _nextClip = new AudioContainer(audioClip, loop);

      //start the fade queue
      PlayNewClip();
    }

    /// <summary>
    /// Mutes all active and future sounds on the layer
    /// </summary>
    /// <param name="mute">True to mute, false to unmute</param>
    public override void MuteLayer(bool mute)
    {
      base.MuteLayer(mute);
      if (_currentAudioSource != null && _currentAudioSource.audioSource != null)
      {
        _currentAudioSource.mute = mute;
      }
    }

    /// <summary>
    /// Pauses all active and future sounds on the layer
    /// </summary>
    /// <param name="pause">True to pause, false to unpause</param>
    public override void PauseLayer(bool pause)
    {
      base.PauseLayer(pause);

      if (_currentAudioSource != null && _currentAudioSource.audioSource != null)
      {
        if (pause)
        {
          _currentAudioSource.Pause();
        }
        else
        {
          _currentAudioSource.Resume();
          PlayNewClip();
        }
      }
    }

    /// <summary>
    /// Fades out the active sound and fades in the next clip
    /// </summary>
    private void PlayNewClip()
    {
      if (_fadeInProgress == false && _pause == false && _nextClip != null)
      {
        _fadeInProgress = true;

        //Cache the clip for use during fade and free up the nextClip in case a new track is queued before this one finishes fading in.
        AudioContainer clip = _nextClip;
        _nextClip = null;

        //If there is existing audio on this layer, cross-fade it with the new clip
        if (_currentAudioSource != null && _currentAudioSource.audioSource != null)
        {
          FadeOutClip(_currentAudioSource, _crossFadeDuration, null);
        }

        _currentAudioSource = clip.audioClip;

        FadeInClip(_currentAudioSource, clip.loop, _crossFadeDuration, delegate()
        {
          _fadeInProgress = false;
          PlayNewClip();
        });
      }
    }

    /// <summary>
    /// Fades in and plays an audio clip for the passed in duration
    /// </summary>
    /// <param name="audioClip">Audio clip to play</param>
    /// <param name="loop">Indicates if the audio clip should loop</param>
    /// <param name="fadeDuration">The duration of the fade</param>
    /// <param name="onComplete">Callback for when the fade is complete</param>
    private void FadeInClip(IAudioSourceController audioClip, bool loop, float fadeDuration, TweenCallback onComplete)
    {
      if (audioClip != null && audioClip.audioSource != null)
      {
        if (_mute)
        {
          audioClip.mute = true;
        }

        float endVolume = audioClip.volume;
        audioClip.volume = 0f;
        audioClip.audioSource.DOFade(endVolume, fadeDuration).OnComplete(onComplete).Play();
        audioClip.Play(loop);

        if (base._pause)
        {
          audioClip.Pause();
        }
      }
      else
      {
        if (onComplete != null)
        {
          onComplete();
        }
      }
    }

    /// <summary>
    /// Fades out and disposes of the audio clip
    /// </summary>
    /// <param name="audioClip">Audio clip to fade out</param>
    /// <param name="fadeDuration">The duration of the fade</param>
    /// <param name="onComplete">Callback for when the fade is complete</param>
    private void FadeOutClip(IAudioSourceController audioClip, float fadeDuration, TweenCallback onComplete)
    {
      if (audioClip != null && audioClip.audioSource != null)
      {
        if (_mute)
        {
          audioClip.mute = true;
        }

        if (base._pause)
        {
          audioClip.Pause();
        }
        audioClip.audioSource.DOFade(0, fadeDuration).OnComplete(delegate()
        {
          audioClip.Dispose();

          if (onComplete != null)
          {
            onComplete();
          }
        }).Play();
      }
      else
      {
        if (onComplete != null)
        {
          onComplete();
        }
      }
    }
  }

  /// <summary>
  /// Used when queueing up a new sound to transition in
  /// </summary>
  class AudioContainer
  {
    /// <summary>
    /// Audio clip controller reference
    /// </summary>
    public IAudioSourceController audioClip;

    /// <summary>
    /// Indicates if this clip should loop
    /// </summary>
    public bool loop;

    /// <summary>
    /// Used when queueing up a new sound to transition in
    /// </summary>
    /// <param name="audioClip">Audio clip controller reference</param>
    /// <param name="loop">Indicates if this clip should loop</param>
    public AudioContainer(IAudioSourceController audioClip, bool loop)
    {
      this.audioClip = audioClip;
      this.loop = loop;
    }
  }
}
