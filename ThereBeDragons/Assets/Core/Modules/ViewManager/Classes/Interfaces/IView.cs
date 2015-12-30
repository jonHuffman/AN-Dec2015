using System;
namespace Core.Module.ViewManagerSystem
{
  public delegate void ViewTransitionComplete();

  public interface IView
  {
    /// <summary>
    /// Used to retrieve the View's assigned ID
    /// </summary>
    IComparable viewID
    {
      get;
    }

    /// <summary>
    /// Used to assign an ID to the view.
    /// </summary>
    /// <param name="viewID">ID of the view.</param>
    void SetViewID(IComparable viewID);

    /// <summary>
    /// Plays any in transition that you may want to set up. onInComplete must be called regardless of whether there is a transition or not.
    /// </summary>
    /// <param name="onInComplete">The function that notifies that the View is does transitioning.</param>
    void TransitionIn(ViewTransitionComplete onInComplete);

    /// <summary>
    /// Plays any out transition that you may want to set up. onOutComplete must be called regardless of whether there is a transition or not.
    /// </summary>
    /// <param name="onOutComplete">The function that notifies that the View is does transitioning.</param>
    void TransitionOut(ViewTransitionComplete onOutComplete);

    /// <summary>
    /// Allows you to update the view with new data using the ViewManger as the intermediary
    /// </summary>
    /// <param name="data">An object containing the data you wish to pass</param>
    void UpdateView(object data);

    /// <summary>
    /// The place to clean up your view before it is destroyed.
    /// </summary>
    void DestroyView();
  }
}
