using System;
using UnityEngine;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// Delegate used to notify a sound has finished playing.
  /// </summary>
  /// <param name="controller">The controller in which the sound is apart of</param>
  public delegate void SoundCompleteHandler(IAudioSourceController controller);

  /// <summary>
  /// Interface for SourceAudio controllers
  /// </summary>
  public interface IAudioSourceController : IDisposable
  {
    /// <summary>
    /// This event that will trigger when the sound is finished playing; Stop() or Pause() will not trigger this event.
    /// </summary>
    event SoundCompleteHandler onSoundComplete;

    /// <summary>
    /// The audio source being controlled by this object.
    /// </summary>
    AudioSource audioSource { get; }

    /// <summary>
    /// The audio clip owned by this object's AudioSource
    /// </summary>
    AudioClip audioClip { set; }

    string soundID { get; set; }
    float volume { get; set; }
    bool loop { set; }
    bool mute { set; }
    bool disposeOnComplete { get; set; }

    /// <summary>
    /// Plays the audio clip. If loop is true, the track will repeat when it reaches the end. 
    /// onSoundComplete will not fire if looping is enabled.
    /// </summary>
    /// <param name="loop">Whether or not you wish the track to loop</param>
    void Play(bool loop);

    /// <summary>
    /// Stops the sound clip. This should not cause the sound to complete playing.
    /// </summary>
    void Stop();

    /// <summary>
    /// Pauses the sound clip. This should not cause the sound to complete playing.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes the sound if it was previsouly paused.
    /// </summary>
    void Resume();
  }
}
