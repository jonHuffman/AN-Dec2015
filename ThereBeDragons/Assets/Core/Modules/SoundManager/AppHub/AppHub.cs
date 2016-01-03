using Core.Module.SoundManagerSystem;
using System;

namespace Core
{
  /// <summary>
  ///AppHub is a partial class that exists with each core system providing access to that system's functionality
  /// </summary>
  /// <remarks>When a core system is imported it brings with it its own version of AppHub. All the seperate versions of AppHub get compiled together into one master service provider.</remarks>
  public partial class AppHub
  {
    private static BaseSoundManager _soundManager;

    public static BaseSoundManager soundManager
    {
      get
      {
        if (_soundManager == null)
        {
#if UNITY_EDITOR
          throw new Exception("The Sound Manager has not been initialized!");
#else
          _soundManager = new NullSoundManager();
#endif
        }
        return _soundManager;
      }
    }
    
    /// <summary>
    /// Set the active Sound Manager for Apphub
    /// </summary>
    /// <param name="soundManager">The sound manager you want the app to use</param>
    public static void SetSoundManager(BaseSoundManager soundManager)
    {
      _soundManager = soundManager;
    }
  }
}
