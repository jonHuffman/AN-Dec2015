using System;
namespace Core.Module.ViewManagerSystem
{
  /// <summary>
  /// A collection of View information, used for view construction
  /// </summary>
	public class ViewInfo : BundleInfo
  {
    public IComparable viewId;
    public IComparable layerId;
    public string viewPath;

    public ViewInfo(IComparable viewID, IComparable layerID, string viewPath, string bundlePath = "") 
      : base(bundlePath)
    {
      this.viewId = viewID;
      this.layerId = layerID;
      this.viewPath = viewPath;
    }
	}
}
