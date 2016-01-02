using UnityEngine;

namespace Core.Data
{
  /// <summary>
  /// A simple collection of core transform values: position, rotation, and scale
  /// </summary>
  public struct TransformData
  {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public TransformData(Vector3 position, Quaternion rotation, Vector3 localScale)
    {
      this.position = position;
      this.rotation = rotation;
      this.localScale = localScale;
    }
  }
}
