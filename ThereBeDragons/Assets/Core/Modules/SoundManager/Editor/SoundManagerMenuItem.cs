using Core.Module.SoundManagerSystem;
using UnityEditor;

namespace Core.Module.SoundManagerSystem
{
  /// <summary>
  /// Adds an 'Edit Sound Bank' option to the Tools menu for ease of access
  /// </summary>
  public static class SoundManagerMenuItem
  {
    /// <summary>
    /// Method that will be invoked when the menu item is selected
    /// </summary>
    [MenuItem("Tools/Edit Sound Bank")]
    public static void EditSoundBank()
    {
      //Sets the loaded scriptable object to be displayed in the inspector
      Selection.activeObject = SoundBank.instance;
    }
  }
}
