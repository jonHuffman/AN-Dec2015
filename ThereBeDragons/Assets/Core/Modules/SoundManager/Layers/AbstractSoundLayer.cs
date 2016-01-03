using System;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// Base class of a sound layer.
  /// </summary>
  abstract class AbstractSoundLayer : IDisposable
  {
    protected bool _mute = false;
    protected bool _pause = false;

    public AbstractSoundLayer() { }

    /// <summary>
    /// Diposes of the sound layer
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    /// Verifies that the max instances of the sound on the layer hasn't been reached.
    /// </summary>
    /// <param name="soundObject">Sound object reference containing max instance data</param>
    /// <returns>True if the sound can be played on the layer</returns>
    public abstract bool WillAcceptSound(SoundObject soundObject);

    /// <summary>
    /// Plays the audio clip.
    /// </summary>
    /// <param name="audioClip">Audio clip controller.</param>
    /// <param name="loop">Whether to loop the sound or not</param>
    public abstract void PlaySound(IAudioSourceController audioClip, bool loop = false);

    /// <summary>
    /// Mutes all active and future sounds on the layer
    /// </summary>
    /// <param name="mute">True to mute, false to unmute</param>
    public virtual void MuteLayer(bool mute)
    {
      _mute = mute;
    }

    /// <summary>
    /// Pauses all active and future sounds on the layer
    /// </summary>
    /// <param name="pause">True to pause, false to unpause</param>
    public virtual void PauseLayer(bool pause)
    {
      _pause = pause;
    }
  }
}
