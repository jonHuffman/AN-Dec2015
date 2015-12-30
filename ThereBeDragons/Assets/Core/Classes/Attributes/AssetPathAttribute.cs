using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This attribute is used to generate a path of an asset. It stores the output
/// as a string but the inspector <see cref="[AssetPathDrawer.cs]"/> shows
/// an object asset. The advantage of this attribute is the fact that the asset
/// can change it's path in resources and the editor would use it's GUID to find 
/// it and get the asset path again. Keep in mind, it will only look for the Object
/// when the inspector is drawn. After all the value is only a string. 
/// </summary>
[AttributeUsage( AttributeTargets.Field ) ] 
public class AssetPathAttribute : PropertyAttribute
{
  public Type assetType;
  public bool requiredInResourcesFolder;

  public AssetPathAttribute(Type aAssetType, bool isRequiredInResourcesFolder)
  {
    assetType = aAssetType;
    requiredInResourcesFolder = isRequiredInResourcesFolder;
  }
}
