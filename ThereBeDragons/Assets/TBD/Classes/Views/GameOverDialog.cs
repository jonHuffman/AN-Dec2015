using Core;
using Core.Module.ViewManagerSystem;
using TBD.Events;
using UnityEngine;
using UnityEngine.UI;

namespace TBD.Views
{
  public class GameOverDialog : BaseView
  {
    private const string BEST_TIME = "bestTime";

    [SerializeField]
    private Animator _animController;
    [SerializeField]
    private Text _timeText;
    [SerializeField]
    private Text _bestTimeText;

    ViewTransitionComplete _onInComplete;
    ViewTransitionComplete _onOutComplete;

    #region BaseView

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

    public override void UpdateView(object data)
    {
      UIData uiData = (UIData)data;

      //If there were more data to be saving I would write this out to a file and save it on device
      int bestTime = PlayerPrefs.GetInt(BEST_TIME);

      if(uiData.time > bestTime)
      {
        bestTime = uiData.time;
        PlayerPrefs.SetInt(BEST_TIME, bestTime);
      }

      _timeText.text = uiData.time.ToString();
      _bestTimeText.text = bestTime.ToString();
    }
    #endregion

    /// <summary>
    /// Handler for the replay button being clicked.
    /// </summary>
    /// <remarks>Linked in the inspector</remarks>
    public void UI_OnReplayClicked()
    {
      AppHub.eventManager.Dispatch(GameEvent.Restart);
      AppHub.viewManager.AddView(View.Start);
      AppHub.viewManager.RemoveView(View.GameOver);
    }

    /// <summary>
    /// Handler for the OnInComplete animation event
    /// </summary>
    /// <remarks>Linked in the inspector</remarks>
    public void ANIM_OnInComplete()
    {
      if(_onInComplete != null)
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