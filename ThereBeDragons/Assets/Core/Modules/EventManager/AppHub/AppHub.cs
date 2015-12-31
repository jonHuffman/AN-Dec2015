using Core.Module.EventManager;
using System;

namespace Core
{
  /// <summary>
  ///AppHub is a partial class that exists with each core system providing access to that system's functionality
  /// </summary>
  /// <remarks>When a core system is imported it brings with it its own version of AppHub. All the seperate versions of AppHub get compiled together into one master service provider.</remarks>
  public partial class AppHub
  {
    private static BaseEventManager _eventManager;

    public static BaseEventManager eventManager
    {
      get
      {
        if(_eventManager == null)
        {
#if UNITY_EDITOR
          throw new Exception("The View Manager has not been initialized!");
#else
          _eventManager = new NullEventManager();
#endif
        }

        return _eventManager;
      }
    }

    /// <summary>
    /// Sets the EVent Manager to be used during this App's lifecycle
    /// </summary>
    /// <param name="eventManager">The event manager to use</param>
    public static void SetEventManager(BaseEventManager eventManager)
    {
      _eventManager = eventManager;
    }
  }
}
