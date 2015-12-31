using System;

namespace Core.Module.EventManager
{
  public class NullEventManager : BaseEventManager
  {
    public NullEventManager()
      : base()
    {

    }

    public override void Register(IEventObserver observer) { }
    public override void Unregister(IEventObserver observer) { }
    public override void Dispatch(IComparable gameEvent, object data = null) { }
  }
}
