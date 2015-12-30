namespace Core.Module.ViewManagerSystem
{
  /// <summary>
  /// The bundle information required for loading Scenes or Views from asset bundles. 
  /// To make use of loading from asset bundles you will need to set the bundle loading function in the ViewManager
  /// </summary>
  public class BundleInfo
  {
    public string bundlePath;
    public bool loadFromBundle;

    public BundleInfo(string bundlePath)
    {
      this.bundlePath = bundlePath;

      if (string.IsNullOrEmpty(this.bundlePath) == false)
      {
        loadFromBundle = true;
      }
    }
  }
}
