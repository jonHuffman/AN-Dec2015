using System;

namespace Core.Module.EventManager
{
  /// <summary>
  /// An observer to be registered with the EventManager for recieving game events
  /// </summary>
  public interface IEventObserver
  {
    void OnNotify(IComparable gameEvent, object data);
  }
}
