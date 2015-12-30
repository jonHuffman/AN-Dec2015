using Core;
using Core.Module.ViewManagerSystem;
using DG.Tweening;
using UnityEngine;

namespace TBD.Views
{
  [RequireComponent(typeof(CanvasGroup))]
  public class StartView : BaseView
  {
    [SerializeField]
    private float _fadeOutDuration = 0.5f;

    private CanvasGroup _group;

    void Awake()
    {
      _group = GetComponent<CanvasGroup>();
    }

    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Space))
      {
        AppHub.viewManager.AddView(View.UI);
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