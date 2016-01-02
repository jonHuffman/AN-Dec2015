using UnityEngine;

namespace TBD
{
  /// <summary>
  /// A simple script that manipulates the UVs of the texture that is managed by the GameObject's attached renderer. Only manipulates the Main Texture
  /// </summary>
  [RequireComponent(typeof(Renderer))]
  public class UVScroll : MonoBehaviour
  {
    [SerializeField]
    private bool _scrollOnX = false;
    [SerializeField]
    private bool _scrollOnY = false;
    [SerializeField]
    private float _xScrollPerSecond = 0.1f;
    [SerializeField]
    private float _yScrollPerSecond = 0.1f;

    private Material _mat;

    void Awake()
    {
      _mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
      Vector2 textureOffset = _mat.GetTextureOffset("_MainTex");

      if (_scrollOnX == true)
      {
        textureOffset.x += _xScrollPerSecond * GameController.Speed * Time.deltaTime;

        if (textureOffset.x > 1)
        {
          textureOffset.x -= 1;
        }
      }

      if (_scrollOnY == true)
      {
        textureOffset.y += _yScrollPerSecond * GameController.Speed * Time.deltaTime;

        if (textureOffset.y > 1)
        {
          textureOffset.y -= 1;
        }
      }

      _mat.SetTextureOffset("_MainTex", textureOffset);
    }
  }
}