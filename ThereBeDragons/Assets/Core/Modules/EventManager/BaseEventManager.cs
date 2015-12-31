using System;

namespace Core.Module.EventManager
{
  /// <summary>
  /// The Event Manager is a single instance class that works with AppHub to prrovide global access.
  /// It handles the communication between unrelated pieces of code, minimizing unnecessary coupling of systems.
  /// </summary>
  /// <remarks>
  /// The Event Manager uses IComparible objects as Event IDs. It is up to you what you use but it is recommended that you use an Enum for readability.
  /// </remarks>
  public abstract class BaseEventManager
  {
    protected static bool _isInstantiated;

    public BaseEventManager()
    {
      if (_isInstantiated)
      {
        throw new Exception("There can only be one instance of the EventManager in the App's lifecycle.");
      }

      _isInstantiated = true;
    }

    /// <summary>
    /// Registers an observer that will be notified when an event is dispatched through the Event Manager
    /// </summary>
    /// <param name="observer">Event Observer to register</param>
    public abstract void Register(IEventObserver observer);

    /// <summary>
    /// Unregisters an observer once it no longer needs to recieve game events
    /// </summary>
    /// <param name="observer">Event Observer to unregister</param>
    public abstract void Unregister(IEventObserver observer);

    /// <summary>
    /// Dispatches a game event to the subscribed observers allowing them to handle it as they see fit
    /// </summary>
    /// <param name="gameEvent">The event to dispatch.</param>
    /// <param name="data">An optional parameter used to pass arbitrary data along with your event.</param>
    public abstract void Dispatch(IComparable gameEvent, object data = null);
  }
}
