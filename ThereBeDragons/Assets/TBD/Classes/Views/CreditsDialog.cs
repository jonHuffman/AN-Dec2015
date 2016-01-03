using Core;
using Core.Module.ViewManagerSystem;
using UnityEngine;

namespace TBD.Views
{
  public class CreditsDialog : BaseView
  {
    [SerializeField]
    private Animator _animController;

    ViewTransitionComplete _onInComplete;
    ViewTransitionComplete _onOutComplete;

    public override void TransitionIn(ViewTransitionComplete onInComplete)
    {
      _onInComplete = onInComplete;

      //For projects with more animations these should be hashed for performance
      _animController.SetTrigger("TransitionIn");
    }

    public override void TransitionOut(ViewTransitionComplete onOutComplete)
    {
      _onOutComplete = onOutComplete;

      //For projects with more animations these should be hashed for performance
      _animController.SetTrigger("TransitionOut");
    }

    /// <summary>
    /// On click handler for the close button
    /// </summary>
    /// <remarks>Linked in inspector</remarks>
    public void UI_OnCloseClicked()
    {
      AppHub.viewManager.RemoveView(View.Credits);
    }

    /// <summary>
    /// Handler for the OnInComplete animation event
    /// </summary>
    /// <remarks>Linked in the inspector</remarks>
    public void ANIM_OnInComplete()
    {
      if (_onInComplete != null)
      {
        _onInComplete();
      }
    }

    /// <summary>
    /// Handler for the OnOutComplete animation event
    /// </summary>
    /// <remarks>Linked in the inspector</remarks>
    public void ANIM_OnOutComplete()
    {
      if (_onOutComplete != null)
      {
        _onOutComplete();
      }
    }
  }
}