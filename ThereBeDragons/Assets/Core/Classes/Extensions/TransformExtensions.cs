using Core.Data;
using UnityEngine;

public static class TransformExtensions
{
  /// <summary>
  /// Gets the basic tranform data stored in a <seealso cref="TransformData"/>
  /// </summary>
  /// <returns>A TransformData object containing the basic transform data</returns>
  public static TransformData GetData(this Transform trans)
  {
    return new TransformData(trans.position, trans.rotation, trans.localScale);
  }
}
