using System;
using System.Collections.Generic;

namespace Core.Module.EventManager
{
  /// <summary>
  /// An inter-system event manager for the application.
  /// Allows for two unrelated pieces of code to communicate without coupling them.
  /// </summary>
  public class EventManager : BaseEventManager
  {
    /// <summary>
    /// The event and data object to send to each observer.
    /// </summary>
    private struct GameEventData
    {
      public GameEventData(IComparable gameEvent, object data)
      {
        this.gameEvent = gameEvent;
        this.data = data;
      }

      public IComparable gameEvent;
      public object data;
    }

    /// <summary>
    /// This delegate defines a logging method for use by the ViewManager. This allows for you to hook up your own logging system shouild you desire.
    /// </summary>
    /// <param name="msg">The message to print.</param>
    public delegate void LogMethod(string msg);

    /// <summary>
    /// Registered observers listening for events.
    /// </summary>
    private List<IEventObserver> _observers;

    /// <summary>
    /// A list of observers that existed when the game event began processing. 
    /// It is possible for an event to cause an observer to be added or removed and we do not want to process these with this event.
    /// </summary>
    private List<IEventObserver> _activeObservers;

    /// <summary>
    /// The events that have been fired by calling the Dispatch function. Each event is fully processed sequencially.
    /// </summary>
    private Queue<GameEventData> _gameEvents;

    private bool _processingEvent = false;

    private LogMethod _log;
    private LogMethod _logWarning;
    private LogMethod _logError;

    public EventManager()
      :base()
    {
      _observers = new List<IEventObserver>();
      _activeObservers = new List<IEventObserver>();
      _gameEvents = new Queue<GameEventData>();
      _processingEvent = false;
    }

    /// <summary>
    /// Registers an observer that will be notified when an event is dispatched through the Event Manager
    /// </summary>
    /// <param name="observer">Event Observer to register</param>
    public override void Register(IEventObserver observer)
    {
      // Remove null observers to prevent issues. Observers can become null if the object is destroyed without being properly cleaned up.
      while (_observers.Contains(null))
      {
        _observers.RemoveAt(_observers.IndexOf(null));
      }

      if(_observers.Contains(observer) == false)
      {
        _observers.Add(observer);

        PrintLog(string.Format("Registered {0}", observer.GetType().Name));
      }
      else
      {
        PrintLogWarning(string.Format("Failed to register {0}, this instance has already been registered.", observer.GetType().Name));
      }
    }

    /// <summary>
    /// Unregisters an observer once it no longer needs to recieve game events
    /// </summary>
    /// <param name="observer">Event Observer to unregister</param>
    public override void Unregister(IEventObserver observer)
    {
      if(_observers.Contains(observer))
      {
        _observers.Remove(observer);

        PrintLog(string.Format("Unregistered {0}", observer.GetType().Name));
      }
#if UNITY_EDITOR
      else
      {
        //We only print this in the editor as failing to unregister observers doesn't really cause an issue.
        PrintLogWarning(string.Format("Failed to unregister {0}, this instance is not registered.", observer.GetType().Name));
      }
#endif

      // If we are in the middle of processing an event we do not want it to notify this observer
      if(_processingEvent == true && _activeObservers.Contains(observer))
      {
        _activeObservers[_activeObservers.IndexOf(observer)] = null;
      }
    }

    /// <summary>
    /// Dispatches a game event to the subscribed observers allowing them to handle it as they see fit
    /// </summary>
    /// <param name="gameEvent">The event to dispatch.</param>
    /// <param name="data">An optional parameter used to pass arbitrary data along with your event.</param>
    public override void Dispatch(IComparable gameEvent, object data = null)
    {
      _gameEvents.Enqueue(new GameEventData(gameEvent, data));

      if(_processingEvent == false)
      {
        DispatchNextEvent();
      }
    }

    /// <summary>
    /// Processes the next event in the Queue notifying the registered observers
    /// </summary>
    private void DispatchNextEvent()
    {
      _processingEvent = true;
      _activeObservers = new List<IEventObserver>(_observers);
      GameEventData nextEvent = _gameEvents.Dequeue();

      PrintLog(string.Format("Dispatching Game Event - {0}", nextEvent.gameEvent.ToString()));

      for (int i = 0; i < _activeObservers.Count; i++)
      {
        //Make sure this observer was not unregistered in the middle of processing
        if (_activeObservers[i] != null)
        {
          _activeObservers[i].OnNotify(nextEvent.gameEvent, nextEvent.data);
        }
      }

      _activeObservers = null;
      _processingEvent = false;

      //If there are more events queued, dispatch them
      if(_gameEvents.Count > 0)
      {
        DispatchNextEvent();
      }
    }

    #region Logging

    /// <summary>
    /// An initialization command that sets the debug methods for use by the event manager. If these methods are not set, the Manager will not output messages.
    /// </summary>
    /// <param name="log">The log function</param>
    /// <param name="logWarning">The log warning function</param>
    /// <param name="logError">The log error function</param>
    public void SetDebugMethods(EventManager.LogMethod log, EventManager.LogMethod logWarning, EventManager.LogMethod logError)
    {
      _log = log;
      _logWarning = logWarning;
      _logError = logError;
    }

    /// <summary>
    /// Outputs a message
    /// </summary>
    /// <param name="msg">Message to output</param>
    private void PrintLog(string msg)
    {
      if (_log != null)
      {
        _log(string.Format("[EventManager] : {0}", msg));
      }
    }

    /// <summary>
    /// Outputs a warning
    /// </summary>
    /// <param name="msg">warning to output</param>
    private void PrintLogWarning(string msg)
    {
      if (_logWarning != null)
      {
        _logWarning(string.Format("[EventManager] : {0}", msg));
      }
    }

    /// <summary>
    /// Outputs an error
    /// </summary>
    /// <param name="msg">Error to output</param>
    private void PrintLogError(string msg)
    {
      if (_logError != null)
      {
        _logError(string.Format("[EventManager] : {0}", msg));
      }
    }
    #endregion
  }
}
