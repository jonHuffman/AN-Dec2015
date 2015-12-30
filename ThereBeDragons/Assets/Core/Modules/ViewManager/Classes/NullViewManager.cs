using System;

namespace Core.Module.ViewManagerSystem
{
  /// <summary>
  /// The null implmentation of the BaseViewManager. 
  /// Used by AppHub in cases when projects do not include manager initialization. This will allow the app to compile though it may not run as intended.
  /// </summary>
  public class NullViewManager : BaseViewManager
  {
    public NullViewManager()
      : base()
    {
    }

    public override void RegisterView(IComparable ID, IComparable layerId, string path) { }
    public override void RegisterView(IComparable ID, IComparable layerId, string name, string bundlePath) { }
    public override void AddView(IComparable ID, object initData = null) { }
    public override void RemoveView(IComparable ID, Action completeCallback = null) { }
    public override void RemoveAllViews() { }
    public override void RemoveAllViews(IComparable[] exemptLayers) { }
    public override void UpdateView(IComparable ID, object data) { }
    public override IComparable GetViewOnLayer(IComparable layer) { return -1; }
    public override bool IsActiveView(IComparable viewID) { return false; }
    public override IComparable[] GetOccupiedLayers() { return new IComparable[0]; }
    public override void ChangeScene(string name, BaseViewManager.OnSceneLoaded loadComplete = null) { }
    public override void ChangeSceneBundle(string bundlePath, OnSceneLoaded loadComplete = null) { }
    public override void SetBundleLoadMethod(BaseViewManager.BundleLoadMethod loadMethod) { }
    public override void ToggleCanvasInput(bool enabled) { }
  }
}
