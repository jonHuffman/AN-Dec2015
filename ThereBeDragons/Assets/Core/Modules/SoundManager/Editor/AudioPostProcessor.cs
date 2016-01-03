using UnityEditor;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// Automatically enables load in background and disables preloading when a new audio file is imported into the project.
  /// </summary>
  public class AudioPostProcessor : AssetPostprocessor
  {
    private void OnPreprocessAudio()
    {
      AudioImporter audio = assetImporter as AudioImporter;
      audio.loadInBackground = true;
      audio.preloadAudioData = false;
    }
  }
}
