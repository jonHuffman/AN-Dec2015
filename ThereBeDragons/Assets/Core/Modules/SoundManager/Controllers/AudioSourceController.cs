using Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// The main controller for individual audio tracks
  /// </summary>
  public class AudioSourceController : MonoBehaviour, IAudioSourceController
  {
    /// <summary>
    /// Keep track of controllers using the same Audio Clip reference allowing us to unload the asset when it is no longer in use. 
    /// </summary>
    /// <remarks>
    /// If a clip is explicitly unloaded while another instance is playing, both will be disposed.
    /// </remarks>
    private static IDictionary<int, int> _referenceCount = new Dictionary<int, int>();

    /// <summary>
    /// This event that will trigger when the sound is finished playing; Stop() or Pause() will not trigger this event.
    /// </summary>
    public event SoundCompleteHandler onSoundComplete;
    
    private AudioSource _source;
    private bool _isPaused;
    private bool _isStopped;

    private IEnumerator _soundMonitor;
    private int _sourceHash = -1;

    #region Properties

    public string soundID { get; set; }
    public bool disposeOnComplete { get; set; }

    /// <summary>
    /// The audio source being controlled by this object.
    /// </summary>
    public AudioSource audioSource
    {
      get { return _source; }
    }

    /// <summary>
    /// The audio clip owned by this object's AudioSource
    /// </summary>
    public AudioClip audioClip 
    {
      set { _source.clip = value; }
    }

    public float volume
    {
      get { return _source.volume; }
      set { _source.volume = value; }
    }

    public bool loop
    {
      set { _source.loop = value; }
    }

    public bool mute
    {
      set { _source.mute = value; }
    }
    #endregion

    private void Awake()
    {
      _soundMonitor = null;
      _source = this.gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Plays the audio clip. If loop is true, the track will repeat when it reaches the end. 
    /// onSoundComplete will not fire if looping is enabled.
    /// </summary>
    /// <param name="loop">Whether or not you wish the track to loop</param>
    public void Play(bool loop)
    {
      _isPaused = false;
      _isStopped = false;

      if (_source.clip != null)
      {
        //Generate the hash and use it to increment the instance counter
        if (_sourceHash == -1)
        {
          _sourceHash = _source.clip.name.GetHashCode();

          if (!_referenceCount.ContainsKey(_sourceHash))
          {
            _referenceCount.Add(_sourceHash, 0);
          }
          _referenceCount[_sourceHash]++;
        }

        this.loop = loop;
        _source.Play();

        if (_soundMonitor == null)
        {
          _soundMonitor = MonitorSoundStatus();
          StartCoroutine(_soundMonitor);
        }
      }
      else if (disposeOnComplete)
      {
        Dispose();
      }
    }

    /// <summary>
    /// Stops the sound clip. This does not cause the sound to complete playing.
    /// </summary>
    /// <remarks>Stopping and pausing are handled seperately by Audio Source, so we keep track of them seperately here.</remarks>
    public void Stop()
    {
      if (!_isStopped)
      {
        _isStopped = true;
        _source.Stop();
      }
    }

    /// <summary>
    /// Pauses the sound clip. This does not cause the sound to complete playing.
    /// </summary>
    /// <remarks>Pausing and stopping are handled seperately by Audio Source, so we keep track of them seperately here.</remarks>
    public void Pause()
    {
      if (!_isPaused)
      {
        _isPaused = true;
        _source.Pause();
      }
    }

    /// <summary>
    /// Resumes the sound if it was previsouly paused.
    /// </summary>
    public void Resume()
    {
      if (_isPaused)
      {
        _isPaused = false;
        _source.UnPause();
      }
    }

    /// <summary>
    /// This coroutine keeps track of the sound clip's status in order to handle clean up when it finishes playing.
    /// </summary>
    /// <returns>Coroutine</returns>
    private IEnumerator MonitorSoundStatus()
    {
      while (_source.clip != null && (_source.isPlaying || _isPaused || _isStopped))
      {
        yield return new WaitForFixedUpdate();
      }

      if (onSoundComplete != null)
      {
        onSoundComplete(this);
      }

      if (disposeOnComplete)
      {
        Dispose();
      }

      _soundMonitor = null;
    }

    /// <summary>
    /// Method called by Unity when the MonoBehaviour is removed from the hierarchy. This method cleans up the AudioSource and unloads the Audio Clip.
    /// </summary>
    private void OnDestroy()
    {
      if (_referenceCount.ContainsKey(_sourceHash))
      {
        _referenceCount[_sourceHash]--;
        if (_source.clip != null && !_source.clip.preloadAudioData && _source.clip.loadState == AudioDataLoadState.Loaded)
        {
          if (_referenceCount[_sourceHash] <= 0)
          {
            _referenceCount.Remove(_sourceHash);
            _source.clip.UnloadAudioData();
          }
        }
      }

      GameObject.Destroy(_source);
    }

    /// <summary>
    /// Stops playback and destroys the AudioSourceController.
    /// </summary>
    public void Dispose()
    {
      Stop();
      StopAllCoroutines();
      GameObject.Destroy(this);
    }
  }
}