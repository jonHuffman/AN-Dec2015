using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// The SoundManager is a single instance class that works with AppHub to provide global access to its functions.
  /// It manages the loading and playing of audio files in your application.
  /// </summary>
  /// <remarks>You could theoretically create two instances of this but it would probably do weird shit.</remarks>
  [RequireComponent(typeof(AudioListener))]
  public class SoundManager : BaseSoundManager
  {
    /// <summary>
    /// This delegate defines a logging method for use by the SoundManager. This allows for you to hook up your own logging system should you desire.
    /// </summary>
    /// <param name="msg">The message to print.</param>
    public delegate void LogMethod(string msg);

    private IDictionary<IComparable, AbstractSoundLayer> _soundLayers;

    private LogMethod _log;
    private LogMethod _logWarning;
    private LogMethod _logError;

    #region Unity

    void Awake()
    {
      DontDestroyOnLoad(this);
      this.gameObject.name = "SoundManager";
      _soundLayers = new Dictionary<IComparable, AbstractSoundLayer>();
    }

    void OnDestroy()
    {
      foreach (KeyValuePair<IComparable, AbstractSoundLayer> kvp in _soundLayers)
      {
        kvp.Value.Dispose();
      }

      _soundLayers.Clear();
      KillAllSounds();
    }
    #endregion

    /// <summary>
    /// Initializes the SoundBank for use are runtime
    /// </summary>
    public override void Initialize()
    {
      SoundBank.instance.Initialize();
    }

    /// <summary>
    /// Gets the volume of the SoundObject multiplied by the master volume.
    /// </summary>
    /// <param name="soundObject">SoundObject to retrieve volume of</param>
    /// <returns>A volume normalized between 0 and 1</returns>
    private float GetVolume(SoundObject soundObject)
    {
      return SoundBank.instance.masterVolume * soundObject.volume;
    }

    /// <summary>
    /// Create a sound layer to use for playing sounds. A layer can be defined as single or multi depending on what you are using the layer for.
    /// </summary>
    /// <param name="layerID">The ID of the layer</param>
    /// <param name="isMultiSound">Will the layer support multiple sounds playing at once</param>
    /// <param name="crossFadeDuration">The fade in and out time of a audio clip on a single sound layer. 0 by default</param>
    public override void CreateLayer(IComparable layerID, bool isMultiSound, float crossFadeDuration = 0f)
    {
      if (_soundLayers.ContainsKey(layerID) == false)
      {
        if (isMultiSound)
        {
          _soundLayers.Add(layerID, new MultiSoundLayer());
        }
        else
        {
          _soundLayers.Add(layerID, new SingleSoundLayer(crossFadeDuration));
        }
      }
      else
      {
        PrintLogWarning(string.Format("Layer with ID {0} has already been created.", layerID));
      }
    }

    /// <summary>
    /// Loads and plays a sound on a specific layer. The layer then controls how the sound will play.
    /// </summary>
    /// <param name="soundID">ID of the sound to load.</param>
    /// <param name="loop">Will the sound loop</param>
    /// <param name="layerID">The layer ID to send the sound to.</param>
    public override void PlaySoundOnLayer(string soundID, bool loop, IComparable layerID)
    {
      if (_soundLayers.ContainsKey(layerID) && _soundLayers[layerID].WillAcceptSound(SoundBank.instance.GetSoundObject(soundID)))
      {
        _soundLayers[layerID].PlaySound(CreateSound(soundID), loop);
      }
    }

    /// <summary>
    /// Mutes all sounds and future sounds on the layer.
    /// </summary>
    /// <param name="layerID">The layer id to mute</param>
    /// <param name="mute">True will mute sounds, false will unmute them</param>
    public override void MuteLayer(IComparable layerID, bool mute)
    {
      if (_soundLayers.ContainsKey(layerID))
      {
        _soundLayers[layerID].MuteLayer(mute);
      }
    }

    /// <summary>
    /// Mutes all sounds on all layers and future played sounds.
    /// </summary>
    /// <param name="mute">True will mute sounds, false will unmute them</param>
    public override void MuteAllLayers(bool mute)
    {
      foreach(KeyValuePair<IComparable, AbstractSoundLayer> kvp in _soundLayers)
      {
        kvp.Value.MuteLayer(mute);
      }
    }

    /// <summary>
    /// Pauses all sounds and future sounds on the layer
    /// </summary>
    /// <param name="layerId">The layer id to pause</param>
    /// <param name="pause">True will pause sounds, false will unpause them</param>
    public override void PauseLayer(IComparable layerId, bool pause)
    {
      if (_soundLayers.ContainsKey(layerId))
      {
        _soundLayers[layerId].PauseLayer(pause);
      }
    }

    /// <summary>
    /// Pauses all active and future sounds on all layers
    /// </summary>
    /// <param name="pause">True will pause sounds, false will unpause them</param>
    public override void PauseAllLayers(bool pause)
    {
      foreach (KeyValuePair<IComparable, AbstractSoundLayer> kvp in _soundLayers)
      {
        kvp.Value.PauseLayer(pause);
      }
    }

    /// <summary>
    /// Stops and disposes all IAudioSourceController objects.
    /// </summary>
    public override void KillAllSounds()
    {
      IAudioSourceController[] activeSounds = GetComponents<IAudioSourceController>();
      if (activeSounds != null && activeSounds.Length > 0)
      {
        for (int i = 0; i < activeSounds.Length; i++)
        {
          activeSounds[i].Dispose();
        }
      }
    }

    /// <summary>
    /// Mutes all IAudioSourceController sounds. Newly created IAudioSourceController objects will also be set to this mute state.
    /// </summary>
    /// <param name="mute">True will mute all sounds, false will unmute them</param>
    public override void MuteAllSounds(bool mute)
    {
      IAudioSourceController[] activeSounds = GetComponents<IAudioSourceController>();
      if (activeSounds != null && activeSounds.Length > 0)
      {
        for (int i = 0; i < activeSounds.Length; i++)
        {
          activeSounds[i].mute = mute;
        }
      }
    }

    /// <summary>
    /// Creates an instance of IAudioSourceController. This object can be used to play, pause, resume and stop a sound.
    /// </summary>
    /// <param name="soundID">The ID of the sound</param>
    /// <param name="disposeOnComplete">Whether to dispose of the IAudioSourceController object when the sound has finished playing.</param>
    /// <returns>An AudioSourceController for the sound ID</returns>
    protected override IAudioSourceController CreateSound(string soundID, bool disposeOnComplete = true)
    {
      IAudioSourceController controller;
      SoundObject soundObject = SoundBank.instance.GetSoundObject(soundID);
      AudioClip clip = SoundBank.instance.GetSoundClip(soundID);

      if (soundObject != null && clip != null)
      {
        controller = this.gameObject.AddComponent<AudioSourceController>();
        controller.soundID = soundID;
        controller.audioClip = clip;
        controller.volume = GetVolume(soundObject);
        controller.disposeOnComplete = disposeOnComplete;
      }
      else
      {
        PrintLogError(string.Format("Sound with ID {0} is malformed or does not exist", soundID));

        controller = new NullAudioSourceController();
      }

      return controller;
    }


    #region Logging

    /// <summary>
    /// An initialization command that sets the debug methods for use by the view manager. If these methods are not set, the Manager will not output messages.
    /// </summary>
    /// <param name="log">The log function</param>
    /// <param name="logWarning">The log warning function</param>
    /// <param name="logError">The log error function</param>
    public void SetDebugMethods(SoundManager.LogMethod log, SoundManager.LogMethod logWarning, SoundManager.LogMethod logError)
    {
      _log = log;
      _logWarning = logWarning;
      _logError = logError;
    }

    /// <summary>
    /// Outputs a message
    /// </summary>
    /// <param name="msg">Message to output</param>
    private void PrintLog(string msg)
    {
      if (_log != null)
      {
        _log(msg);
      }
    }

    /// <summary>
    /// Outputs a warning
    /// </summary>
    /// <param name="msg">warning to output</param>
    private void PrintLogWarning(string msg)
    {
      if (_logWarning != null)
      {
        _logWarning(msg);
      }
    }

    /// <summary>
    /// Outputs an error
    /// </summary>
    /// <param name="msg">Error to output</param>
    private void PrintLogError(string msg)
    {
      if (_logError != null)
      {
        _logError(msg);
      }
    }
    #endregion
  }
}
