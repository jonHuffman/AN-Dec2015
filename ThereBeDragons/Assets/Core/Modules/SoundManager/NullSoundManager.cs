using System;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// The null implmentation of the BaseSoundManager
  /// Used by AppHub to avoid runtime errors in the event that a proper manager not set
  /// </summary>
  public class NullSoundManager : BaseSoundManager
  {
    public override void Initialize() { }
    public override void CreateLayer(IComparable layerId, bool isMultiSound, float crossFadeDuration = 0f) { }
    public override void PlaySoundOnLayer(string soundId, bool loop, IComparable layerId) { }
    public override void MuteLayer(IComparable layerId, bool mute) { }
    public override void MuteAllLayers(bool mute) { }
    public override void PauseLayer(IComparable layerId, bool pause) { }
    public override void PauseAllLayers(bool pause) { }
    public override void KillAllSounds() { }
    public override void MuteAllSounds(bool mute) { }
    protected override IAudioSourceController CreateSound(string id, bool disposeOnComplete = true)
    {
      return new NullAudioSourceController();
    }
  }
}