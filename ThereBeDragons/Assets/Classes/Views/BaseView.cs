using Core.Module.ViewManagerSystem;
using UnityEngine;

namespace TBD.Views
{
  /// <summary>
  /// The most basic implmentation of a View that is usable by the view manager
  /// </summary>
  public abstract class BaseView : MonoBehaviour, IView
  {
    #region IView

    public System.IComparable viewID { get; private set; }

    public virtual void SetViewID(System.IComparable viewID)
    {
      this.viewID = viewID;
    }

    public virtual void TransitionIn(ViewTransitionComplete onInComplete)
    {
      onInComplete();
    }

    public virtual void TransitionOut(ViewTransitionComplete onOutComplete)
    {
      onOutComplete();
    }

    public virtual void UpdateView(object data) { }

    public virtual void DestroyView()
    {
      Destroy(gameObject);
    }
    #endregion
  }
}
