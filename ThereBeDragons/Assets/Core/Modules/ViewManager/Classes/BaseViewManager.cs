using System;
using System.Collections.Generic;

namespace Core.Module.ViewManagerSystem
{
  /// <summary>
  /// The View Manager is a single instance class that works with AppHub to provide global access. 
  /// It handles the creation and lifecycle of all UI based views within a game as well as some basic scene management functionality.
  /// </summary>
  /// <remarks>
  /// The base implmentation of the ViewManager. Provides a core set of functionality that all ViewManagers need to support. 
  /// If you would like to write a new version of the ViewManager please make sure to extend this base class.
  /// </remarks>
  public abstract class BaseViewManager
  {
    public delegate void OnSceneLoaded();
    public delegate void OnBundleLoaded(BundleInfo bundleInfo, Object loadedObj);
    public delegate void BundleLoadMethod(BundleInfo bundleInfo, OnBundleLoaded onBundleLoaded);
    public delegate void ViewChangedMethod(IComparable ID);

    public event ViewChangedMethod onViewOpened;
    public event ViewChangedMethod onViewClosed;

    protected static bool _isInstantiated;
    /// <summary>
    /// layer ID, view
    /// </summary>
    protected Dictionary<IComparable, IView> _activeViews = new Dictionary<IComparable, IView>();

    public BaseViewManager()
    {
      if (_isInstantiated)
      {
        throw new Exception("There can only be one instance of the ViewManager in the App's lifecycle.");
      }
      _isInstantiated = true;
    }

    protected void DispatchViewOpened(IComparable ID)
    {
      if (onViewOpened != null)
      {
        onViewOpened(ID);
      }
    }

    protected void DispatchViewClosed(IComparable ID)
    {
      if (onViewClosed != null)
      {
        onViewClosed(ID);
      }
    }

    public abstract void RegisterView(IComparable ID, IComparable layerId, string path);
    public abstract void RegisterView(IComparable ID, IComparable layerId, string name, string bundlePath);
    public abstract void AddView(IComparable ID, object initData = null);
    public abstract void RemoveView(IComparable ID, Action completeCallback = null);
    public abstract void RemoveAllViews();
    public abstract void RemoveAllViews(IComparable[] exemptLayers);
    public abstract void UpdateView(IComparable ID, object data);
    public abstract IComparable GetViewOnLayer(IComparable layer);
    public abstract bool IsActiveView(IComparable viewID);
    public abstract IComparable[] GetOccupiedLayers();
    public abstract void ChangeScene(string name, OnSceneLoaded loadComplete = null);
    public abstract void ChangeSceneBundle(string bundlePath, OnSceneLoaded loadComplete = null);
    public abstract void SetBundleLoadMethod(BundleLoadMethod loadMethod);
    public abstract void ToggleCanvasInput(bool enabled);
  }
}
