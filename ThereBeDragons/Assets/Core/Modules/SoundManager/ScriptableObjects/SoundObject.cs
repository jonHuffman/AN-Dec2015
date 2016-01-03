using System;
using System.IO;
using UnityEngine;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// Contains data about a sound file.
  /// </summary>
  [Serializable]
  public class SoundObject
  {
    [SerializeField]
    private string _audioID = string.Empty;

    [SerializeField, AssetPathAttribute(typeof(AudioClip), true)]
    private string _audioFile = string.Empty;

    [SerializeField, Range(0, 1)]
    private float _volume = 1f;

    [SerializeField]
    private int _maxInstances = 99;

    private string _audioFilename = null;

    #region Properties

    /// <summary>
    /// The ID of this object
    /// </summary>
    public string audioID { get { return _audioID; } }

    /// <summary>
    /// The volume for this specific SoundClip. Value between 0 and 1
    /// </summary>
    public float volume { get { return _volume; } }

    /// <summary>
    /// The amount of instances of this clip that can be played at once
    /// </summary>
    public int maxInstances { get { return _maxInstances; } }
    #endregion

    /// <summary>
    /// Returns the resource name of the SoundClip
    /// </summary>
    /// <returns>Resource name of the clip</returns>
    public string GetAudioFileName()
    {
      if (string.IsNullOrEmpty(_audioFilename))
      {
        _audioFilename = Path.GetFileNameWithoutExtension(_audioFile);

        //Since we use the AssetPathAttribute to link audio assets into the sound bank we need to strip the filename to load from the Resources folder
        string[] split = _audioFile.Split('/');

        if (split != null && split.Length > 1)
        {
          //Traverse the file path backwards until we hit the Resources folder
          for (int i = split.Length - 2; i >= 0; i--)
          {
            if (string.Compare(split[i], "Resources") != 0)
            {
              _audioFilename = string.Concat(split[i] + "/", _audioFilename);
              continue;
            }
            break;
          }
        }
      }
      return _audioFilename;
    }
  }
}
