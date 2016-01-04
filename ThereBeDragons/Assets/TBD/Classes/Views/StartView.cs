using Core;
using Core.Module.ViewManagerSystem;
using Core.Utils;
using DG.Tweening;
using TBD.Events;
using UnityEngine;
using UnityEngine.UI;

namespace TBD.Views
{
  [RequireComponent(typeof(CanvasGroup))]
  public class StartView : BaseView
  {
    [SerializeField]
    private float _fadeOutDuration = 0.5f;
    [SerializeField]
    private Text _actionText;

    private CanvasGroup _group;
    private bool _gameStarted = false;
    private int _tweenID;

    void Awake()
    {
      _group = GetComponent<CanvasGroup>();
      AppHub.soundManager.PlaySoundOnLayer(AudioID.MainMenu, true, SoundLayers.Music);

#if UNITY_IPHONE || UNITY_ANDROID
      _actionText.text = "TAP to start!";
#else
      _actionText.text = "Press SPACE to start!";
#endif

      PulseTextIn();
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

    void OnDestroy()
    {
      DOTween.Kill(_tweenID);
    }

    /// <summary>
    /// Fades the elemtns in this View to an alpha of 0 and then notifies the View Manager that the transition is complete
    /// </summary>
    /// <param name="onOutComplete">Callback that is to be fired once the transition out is complete</param>
    public override void TransitionOut(ViewTransitionComplete onOutComplete)
    {
      _group.DOFade(0f, _fadeOutDuration).OnComplete(() => { onOutComplete(); });
    }

    /// <summary>
    /// Scales the action text up
    /// </summary>
    private void PulseTextIn()
    {
      _tweenID = Helpers.UniqueID;
      _actionText.transform.DOScale(1.1f, 1f).SetId(_tweenID).SetEase(Ease.InOutSine).OnComplete(PulseTextOut);
    }

    /// <summary>
    /// Scales the action text down
    /// </summary>
    private void PulseTextOut()
    {
      _tweenID = Helpers.UniqueID;
      _actionText.transform.DOScale(1f, 1f).SetId(_tweenID).SetEase(Ease.InOutSine).OnComplete(PulseTextIn);
    }
  }
}