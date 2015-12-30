using Core.Module.ViewManagerSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TBD.Views
{
  [RequireComponent(typeof(CanvasGroup))]
  public class UIView : BaseView
  {
    /// <summary>
    /// A samll data object for use by UIView's update function
    /// </summary>
    public struct UIData
    {
      public int distance;
      public int attempt;
    }

    [SerializeField]
    private Text _distanceText;
    [SerializeField]
    private Text _attemptText;
    [SerializeField]
    private float _fadeDuration = 0.5f;

    private CanvasGroup _group;

    void Awake()
    {
      _group = GetComponent<CanvasGroup>();
    }
    
    /// <summary>
    /// Fades the elements in this View to an alpha of 1 and then notifies the View Manager that the transition is complete
    /// </summary>
    /// <param name="onOutComplete">Callback that is to be fired once the transition in is complete</param>
    public override void TransitionIn(ViewTransitionComplete onInComplete)
    {
      _group.DOFade(1f, _fadeDuration).OnComplete(() => { onInComplete(); });
    }

    /// <summary>
    /// Fades the elements in this View to an alpha of 0 and then notifies the View Manager that the transition is complete
    /// </summary>
    /// <param name="onOutComplete">Callback that is to be fired once the transition out is complete</param>
    public override void TransitionOut(ViewTransitionComplete onOutComplete)
    {
      _group.DOFade(0f, _fadeDuration).OnComplete(() => { onOutComplete(); });
    }

    /// <summary>
    /// Updates the distance and attempts text in the UI
    /// </summary>
    /// <param name="data">A UIView.UIData object containing player progress data</param>
    public override void UpdateView(object data)
    {
      UIData uiData = (UIData)data;

      _distanceText.text = uiData.distance.ToString();
      _attemptText.text = uiData.attempt.ToString();
    }
  }
}
