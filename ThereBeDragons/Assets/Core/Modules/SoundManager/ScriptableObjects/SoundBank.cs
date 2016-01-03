using System.Collections.Generic;
using UnityEngine;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// A scriptable object that stores and organizes SoundObject data.
  /// </summary>
  public class SoundBank : ScriptableObject
  {
    private static SoundBank _instance = null;

    /// <summary>
    /// Automatically locates and returns the SoundBank object in your SoundManager's resources folder
    /// </summary>
    public static SoundBank instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = Resources.Load<SoundBank>("SoundBank");
        }
        return _instance;
      }
    }

    /// <summary>
    /// A master volume control for all sounds managed by this manager
    /// </summary>
    [SerializeField, Range(0, 1)]
    private float _masterVolume = 1f;

    /// <summary>
    /// List of SoundObjects data.
    /// </summary>
    [SerializeField]
    private List<SoundObject> _sounds = null;

    /// <summary>
    /// Used to quickly grab SoundObjects used by the game.
    /// </summary>
    private IDictionary<string, SoundObject> _bank = new Dictionary<string, SoundObject>();

    #region Properties

    /// <summary>
    /// The master volume normalized between 0 and 1
    /// </summary>
    public float masterVolume { get { return _masterVolume; } }
    #endregion

    /// <summary>
    /// Generates a dictionary for quick lookup of sounds at runtime
    /// </summary>
    public void Initialize()
    {
      _bank.Clear();

      for (int i = 0; i < _sounds.Count; i++)
      {
        if (!_bank.ContainsKey(_sounds[i].audioID))
        {
          _bank.Add(_sounds[i].audioID, _sounds[i]);
        }
      }
    }

    /// <summary>
    /// Returns a SoundObject which contains data about a sound file. Returns null if not found.
    /// </summary>
    /// <param name="soundId">id of the SoundObject.</param>
    /// <returns>SoundObject containing sound data.</returns>
    public SoundObject GetSoundObject(string soundId)
    {
      if (_bank.ContainsKey(soundId))
      {
        return _bank[soundId];
      }
      return null;
    }

    /// <summary>
    /// Returns the Audio Clip associated with the sound ID. Returns null if not found.
    /// </summary>
    /// <param name="soundId">ID of the SoundObject</param>
    /// <returns>Audio Clip</returns>
    public AudioClip GetSoundClip(string soundId)
    {
      SoundObject soundObject = GetSoundObject(soundId);
      if (soundObject != null)
      {
        return Resources.Load<AudioClip>(soundObject.GetAudioFileName());
      }
      return null;
    }
  }
}