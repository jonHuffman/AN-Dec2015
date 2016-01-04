using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toggles teh sprite on a button
/// </summary>
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class ToggleButton : MonoBehaviour
{
  [SerializeField]
  private Sprite _on;
  [SerializeField]
  private Sprite _off;
  [SerializeField]
  private bool _onByDefault = true;

  private Image _buttonImage;

  void Awake()
  {
    _buttonImage = GetComponent<Image>();
    GetComponent<Button>().onClick.AddListener(OnButtonClick);

    if(_onByDefault == true)
    {
      _buttonImage.sprite = _on;
    }
    else
    {
      _buttonImage.sprite = _off;
    }
  }

  /// <summary>
  /// On click handler for the toggle button
  /// </summary>
  private void OnButtonClick()
  {
    _buttonImage.sprite = (_buttonImage.sprite == _on) ? _off : _on;
  }
}
