using System;
using UnityEngine;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// The SoundManager is a single instance class that works with AppHub to provide global access to its functions.
  /// It manages the loading and playing of audio files in your application.
  /// </summary>
  public abstract class BaseSoundManager : MonoBehaviour
  {    
    /// <summary>
    /// Initializes the SoundBank for use are runtime
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Create a sound layer to use for playing sounds. A layer can be defined as single or multi depending on what you are using the layer for.
    /// </summary>
    /// <param name="layerID">The ID of the layer</param>
    /// <param name="isMultiSound">Will the layer support multiple sounds playing at once</param>
    /// <param name="crossFadeDuration">The fade in and out time of a audio clip on a single sound layer. 0 by default</param>
    public abstract void CreateLayer(IComparable layerId, bool isMultiSound, float crossFadeDuration = 0f);

    /// <summary>
    /// Loads and plays a sound on a specific layer. The layer then controls how the sound will play.
    /// </summary>
    /// <param name="soundID">ID of the sound to load.</param>
    /// <param name="loop">Will the sound loop</param>
    /// <param name="layerID">The layer ID to send the sound to.</param>
    public abstract void PlaySoundOnLayer(string soundId, bool loop, IComparable layerId);
    
    /// <summary>
    /// Mutes all sounds and future sounds on the layer.
    /// </summary>
    /// <param name="layerID">The layer id to mute</param>
    /// <param name="mute">True will mute sounds, false will unmute them</param>
    public abstract void MuteLayer(IComparable layerId, bool mute);
    
    /// <summary>
    /// Mutes all sounds on all layers and future played sounds.
    /// </summary>
    /// <param name="mute">True will mute sounds, false will unmute them</param>
    public abstract void MuteAllLayers(bool mute);

    /// <summary>
    /// Pauses all sounds and future sounds on the layer
    /// </summary>
    /// <param name="layerId">The layer id to pause</param>
    /// <param name="pause">True will pause sounds, false will unpause them</param>
    public abstract void PauseLayer(IComparable layerId, bool pause);

    /// <summary>
    /// Pauses all active and future sounds on all layers
    /// </summary>
    /// <param name="pause">True will pause sounds, false will unpause them</param>
    public abstract void PauseAllLayers(bool pause);

    /// <summary>
    /// Stops and disposes all IAudioSourceController objects.
    /// </summary>
    public abstract void KillAllSounds();

    /// <summary>
    /// Mutes all IAudioSourceController sounds. Newly created IAudioSourceController objects will also be set to this mute state.
    /// </summary>
    /// <param name="mute">True will mute all sounds, false will unmute them</param>
    public abstract void MuteAllSounds(bool mute);

    /// <summary>
    /// Creates an instance of IAudioSourceController. This object can be used to play, pause, resume and stop a sound.
    /// </summary>
    /// <param name="soundID">The ID of the sound</param>
    /// <param name="disposeOnComplete">Whether to dispose of the IAudioSourceController object when the sound has finished playing.</param>
    /// <returns>An AudioSourceController for the sound ID</returns>
    protected abstract IAudioSourceController CreateSound(string id, bool disposeOnComplete = true);
  }
}
