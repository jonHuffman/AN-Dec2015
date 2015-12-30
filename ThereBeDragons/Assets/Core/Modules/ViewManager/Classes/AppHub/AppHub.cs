using Core.Module.ViewManagerSystem;
using System;

namespace Core
{
  /// <summary>
  ///AppHub is a partial class that exists with each core system providing access to that system's functionality
  /// </summary>
  /// <remarks>When a core system is imported it brings with it its own version of AppHub. All the seperate versions of AppHub get compiled together into one master service provider.</remarks>
  public partial class AppHub
  {
    private static BaseViewManager _viewManager;

    public static BaseViewManager viewManager
    {
      get
      {
        if (_viewManager == null)
        {
#if UNITY_EDITOR
          throw new Exception("The View Manager has not been initialized!");
#else
          _viewManager = new NullViewManager();
#endif
        }
        return _viewManager;
      }
    }

    /// <summary>
    /// Set the active View Manager for Apphub
    /// </summary>
    /// <param name="viewManager">The view manager you want the app to use</param>
    public static void SetViewManager(BaseViewManager viewManager)
    {
      _viewManager = viewManager;
    }
  }
}
