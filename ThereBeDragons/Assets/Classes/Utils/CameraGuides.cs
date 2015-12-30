using UnityEngine;

namespace TBD.Utils
{
  /// <summary>
  /// A basic guide class that renders a wireframe cube in the position of the camera so that it is visible at all times.
  /// Only support orthographic cameras.
  /// </summary>
  [RequireComponent(typeof(Camera))]
  public class CameraGuides : MonoBehaviour
  {
    private Camera _camera;

    void OnDrawGizmos()
    {
      if (_camera == null)
      {
        _camera = GetComponent<Camera>();
      }

      if (_camera.orthographic == true)
      {
        Vector3 center = _camera.transform.position;
        center.z += ((_camera.farClipPlane - _camera.nearClipPlane) / 2) + _camera.nearClipPlane;

        Vector3 size = Vector3.zero;
        size.y = _camera.orthographicSize * 2;
        size.x = size.y * _camera.aspect;
        size.z = _camera.farClipPlane - _camera.nearClipPlane;

        Gizmos.DrawWireCube(center, size);
      }
    }
  }
}
