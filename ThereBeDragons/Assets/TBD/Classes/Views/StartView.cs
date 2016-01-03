using Core;
using Core.Module.ViewManagerSystem;
using DG.Tweening;
using TBD.Events;
using UnityEngine;

namespace TBD.Views
{
  [RequireComponent(typeof(CanvasGroup))]
  public class StartView : BaseView
  {
    [SerializeField]
    private float _fadeOutDuration = 0.5f;

    private CanvasGroup _group;
    private bool _gameStarted = false;

    void Awake()
    {
      _group = GetComponent<CanvasGroup>();
      AppHub.soundManager.PlaySoundOnLayer("MainMenu", true, SoundLayers.Music);
    }

    void Update()
    {
      if ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && _gameStarted == false)
      {
        _gameStarted = true;
        AppHub.viewManager.AddView(View.UI);
        AppHub.eventManager.Dispatch(GameEvent.StartGame);
      }
    }

    /// <summary>
    /// Fades the elemtns in this View to an alpha of 0 and then notifies the View Manager that the transition is complete
    /// </summary>
    /// <param name="onOutComplete">Callback that is to be fired once the transition out is complete</param>
    public override void TransitionOut(ViewTransitionComplete onOutComplete)
    {
      _group.DOFade(0f, _fadeOutDuration).OnComplete(() => { onOutComplete(); });
    }
  }
}