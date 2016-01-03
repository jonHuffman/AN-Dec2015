using UnityEngine;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// The NullAudioSourceController exists to ensure that if there is an issue accessing a sound from the Sound Bank that it is gracefully handled instead of throwing an exception.
  /// </summary>
  /// <remarks>This will become more valuable with the addition of asset bundle support</remarks>
  public class NullAudioSourceController : IAudioSourceController
  {
    public event SoundCompleteHandler onSoundComplete;
    public AudioSource audioSource { get { return null; } }
    public string soundID { get; set; }
    public AudioClip audioClip { set { } }
    public float volume { get; set; }
    public bool loop { set { } }
    public bool mute { set { } }
    public bool disposeOnComplete { get; set; }
    public void Play(bool loop)
    {
      if (onSoundComplete != null)
      {
        onSoundComplete(this);
      }
    }
    public void Stop() { }
    public void Pause() { }
    public void Resume()
    {
      if (onSoundComplete != null)
      {
        onSoundComplete(this);
      }
    }
    public void Dispose() { }
  }
}
