using System.Collections.Generic;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// A sound layer that supports multiple clips
  /// </summary>
  class MultiSoundLayer : AbstractSoundLayer
  {
    /// <summary>
    /// Active sounds on the layer.
    /// </summary>
    private IList<IAudioSourceController> _audioClips;

    private IDictionary<string, int> _instaceTracker;

    public MultiSoundLayer()
      : base()
    {
      _audioClips = new List<IAudioSourceController>();
      _instaceTracker = new Dictionary<string, int>();
    }

    /// <summary>
    /// Diposes of the sound layer
    /// </summary>
    public override void Dispose()
    {
      for (int i = 0; i < _audioClips.Count; i++)
      {
        _audioClips[i].onSoundComplete -= OnSoundComplete;
      }

      _audioClips.Clear();
      _audioClips = null;
      _instaceTracker.Clear();
      _instaceTracker = null;
    }

    /// <summary>
    /// Verifies that the max instances of the sound on the layer hasn't been reached.
    /// </summary>
    /// <param name="soundObject">Sound object reference containing max instance data</param>
    /// <returns>True if the sound can be played on the layer</returns>
    public override bool WillAcceptSound(SoundObject soundObject)
    {
      if (soundObject != null && _instaceTracker.ContainsKey(soundObject.audioID))
      {
        if (_instaceTracker[soundObject.audioID] >= soundObject.maxInstances)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Plays the audio clip while taking this layer's setting into account.
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

      if (_instaceTracker.ContainsKey(audioClip.soundID) == false)
      {
        _instaceTracker.Add(audioClip.soundID, 0);
      }

      _instaceTracker[audioClip.soundID]++;
      _audioClips.Add(audioClip);

      if (_mute)
      {
        audioClip.mute = true;
      }

      audioClip.onSoundComplete += OnSoundComplete;
      audioClip.Play(loop);

      //Need to start playing before we can pause
      if (_pause)
      {
        audioClip.Pause();
      }
    }

    /// <summary>
    /// Mutes all active and future sounds on the layer
    /// </summary>
    /// <param name="mute">True to mute, false to unmute</param>
    public override void MuteLayer(bool mute)
    {
      base.MuteLayer(mute);

      for (int i = 0; i < _audioClips.Count; ++i)
      {
        if (_audioClips[i] != null)
        {
          _audioClips[i].mute = mute;
        }
      }
    }

    /// <summary>
    /// Pauses all active and future sounds on the layer
    /// </summary>
    /// <param name="pause">True to pause, false to unpause</param>
    public override void PauseLayer(bool pause)
    {
      base.PauseLayer(pause);

      for (int i = 0; i < _audioClips.Count; ++i)
      {
        if (_audioClips[i] != null)
        {
          if (pause)
          {
            _audioClips[i].Pause();
          }
          else
          {
            _audioClips[i].Resume();
          }
        }
      }
    }

    /// <summary>
    /// Removes the sound from the active sound list
    /// </summary>
    /// <param name="controller">Sound that has completed or been disposed</param>
    private void OnSoundComplete(IAudioSourceController controller)
    {
      controller.onSoundComplete -= OnSoundComplete;

      if (_audioClips.Contains(controller))
      {
        _audioClips.Remove(controller);
      }

      if (_instaceTracker.ContainsKey(controller.soundID))
      {
        _instaceTracker[controller.soundID]--;
      }
    }
  }
}
