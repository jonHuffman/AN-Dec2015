using Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Module.ViewManagerSystem
{
  /// <summary>
  /// The View Manager is a single instance class that works with AppHub to provide global access. 
  /// It handles the creation and lifecycle of all UI based views within a game as well as some basic scene management functionality.
  /// </summary>
  /// <remarks> The default implmentation of the ViewManager</remarks>
  public class ViewManager : BaseViewManager
  {
    /// <summary>
    /// This delegate defines a logging method for use by the ViewManager. This allows for you to hook up your own logging system shouild you desire.
    /// </summary>
    /// <param name="msg">The message to print.</param>
    public delegate void LogMethod(string msg);

    private Canvas _viewCanvas;
    private CanvasGroup _canvasGroup;
    private List<IComparable> _layers = new List<IComparable>();
    /// <summary>
    /// layer ID, layer name
    /// </summary>
    private Dictionary<IComparable, string> _layerNames = new Dictionary<IComparable, string>();
    /// <summary>
    /// view ID, view info
    /// </summary>
    private Dictionary<IComparable, ViewInfo> _registeredViews = new Dictionary<IComparable, ViewInfo>();

    private GameObject _sceneLoadHandler;
    private BaseViewManager.OnSceneLoaded _onSceneLoadComplete;
    private BundleLoadMethod _bundleLoadMethod;
    private bool _loadingScene = false;

    // Collections tracking views currently being added/removed, to allow ignoring duplicate calls
    private HashSet<IComparable> _viewsAddList = new HashSet<IComparable>();
    private HashSet<IComparable> _viewsRemoveList = new HashSet<IComparable>();

#region Greyout layer variables
    private const float TRANSITION_TIMESTEP = 0.0083f;
    private float _greyoutTargetAlpha = 0.3f;
    private Image _greyoutImage;
    private List<IComparable> _overlayLayers;
    private System.Collections.IEnumerator _greyoutTransition;
#endregion

    // Tracks the number of elements disabling input, so that they may stack
    private int _inputDisableCounter = 0;

    private LogMethod _log;
    private LogMethod _logWarning;
    private LogMethod _logError;

    public ViewManager()
      : base()
    {
      _sceneLoadHandler = new GameObject();
      _sceneLoadHandler.hideFlags = HideFlags.HideInHierarchy;
      UnityEngine.Object.DontDestroyOnLoad(_sceneLoadHandler);
      SceneLoadHandler sceneLoadHandler = _sceneLoadHandler.AddComponent<SceneLoadHandler>();
      sceneLoadHandler.sceneLoadComplete += new SceneLoadHandler.SceneLoadCompleteHandler(OnSceneLoadComplete);
    }

    /// <summary>
    /// Registers a View to be used by the View Manager. Once a view is registered it can be added to the canvas by using AddView.
    /// </summary>
    /// <param name="ID">ID of the view to add.</param>
    /// <param name="layerId">The layer that the view should belong to.</param>
    /// <param name="path">The path to the view prefab relative to a Resources folder.</param>
    public override void RegisterView(IComparable ID, IComparable layerId, string path)
    {
      // Throw an exception if we are attempting to register a view using a pre-existing ID.
      if (_registeredViews.ContainsKey(ID))
      {
        throw new System.InvalidOperationException(string.Format("A View with ID {0} has already been registered. This new view will not be registered.", ID));
      }

      ViewInfo viewInfo = new ViewInfo(ID, layerId, path);

      _registeredViews.Add(ID, viewInfo);
      RegisterLayer(layerId);
    }

    /// <summary>
    /// Registers a View to be used by the View Manager. Once a view is registered it can be added to the canvas by using AddView.
    /// </summary>
    /// <param name="ID">ID of the view to add.</param>
    /// <param name="layerId">The layer that the view should belong to.</param>
    /// <param name="name">Name of the prefab that exists in the asset bundle.</param>
    /// <param name="bundlePath">The path to the asset bundle that needs to be loaded for this view.</param>
    public override void RegisterView(IComparable ID, IComparable layerId, string name, string bundlePath)
    {
      // Throw an exception if we are attempting to register a view using a pre-existing ID.
      if (_registeredViews.ContainsKey(ID))
      {
        throw new System.InvalidOperationException(string.Format("A View with ID {0} has already been registered. This new view will not be registered.", ID));
      }

      ViewInfo viewInfo = new ViewInfo(ID, layerId, name, bundlePath);

      _registeredViews.Add(ID, viewInfo);
      RegisterLayer(layerId);
    }

    /// <summary>
    /// Adds a view to the canvas at the correct layer. If there is already a view occupying that layer it will be removed.
    /// </summary>
    /// <param name="ID">ID of the view to add.</param>
    /// <param name="initData">A set of data to be passed to via UpdateView after the view is created</param>
    public override void AddView(IComparable ID, object initData = null)
    {
      if (!_registeredViews.ContainsKey(ID))
      {
        PrintLogWarning(string.Format("No View with ID {0} has been registered. No View will be shown.", ID));
        return;
      }

      ViewInfo view = _registeredViews[ID];

      //Check to see if there is already a view on the specified layer and handle the situation accordingly.
      IView viewOnLayer = _activeViews[view.layerId];
      if (viewOnLayer != null)
      {
        //check if a copy of the view is already in view or in the queue to be shown
        if (viewOnLayer.viewID == view.viewId || _viewsAddList.Contains(view.viewId))
        {
          PrintLog(string.Format("A view with ID {0} already exists in the scene.", view.viewId));
          return;
        }

        //add the view id to the add queue.
        _viewsAddList.Add(view.viewId);

        viewOnLayer.TransitionOut(delegate
        {
          _activeViews[view.layerId] = null;
          viewOnLayer.DestroyView();
          UnityEngine.Object.Destroy((viewOnLayer as Component).gameObject);
          CreateAndAddView(view);

          //dispatch view opened event
          DispatchViewOpened(view.viewId);

          if (initData != null)
          {
            UpdateView(ID, initData);
          }
          //remove view since it is now the active view on the layer
          _viewsAddList.Remove(view.viewId);
        });
      }
      else
      {
        CreateAndAddView(view);

        //dispatch view opened event
        DispatchViewOpened(view.viewId);

        if (initData != null)
        {
          UpdateView(ID, initData);
        }
      }
    }

    /// <summary>
    /// Removes a view from the view canvas
    /// </summary>
    /// <param name="ID">ID of the view to remove.</param>
    /// <param name="completeCallback">Fired on transition out</param>
    public override void RemoveView(IComparable ID, System.Action completeCallback = null)
    {
      if (!_registeredViews.ContainsKey(ID))
      {
        PrintLogWarning(string.Format("No View with ID {0} has been registered. No View will be removed.", ID));
        return;
      }

      ViewInfo view = _registeredViews[ID];

      IView viewOnLayer = _activeViews[view.layerId];

      //Ensure that the view currently active on the layer is the one that is trying to be removed, and is not already being removed by a previous call
      if (viewOnLayer != null && view.viewId == viewOnLayer.viewID && !_viewsRemoveList.Contains(ID))
      {
        _viewsRemoveList.Add(ID);

        viewOnLayer.TransitionOut(delegate
        {
          //view closed event
          DispatchViewClosed(ID);

          _viewsRemoveList.Remove(ID);

          viewOnLayer.DestroyView();
          UnityEngine.Object.Destroy((viewOnLayer as Component).gameObject);

          //Confirm that a new view on the same layer has not been added in the middle of processing the removal of this view
          if (viewOnLayer.viewID == _activeViews[view.layerId].viewID)
          {
            _activeViews[view.layerId] = null;
          }

          if (completeCallback != null)
          {
            completeCallback();
          }

          UpdateGreyoutPosition(view.layerId);
        });
      }
      else
      {
        PrintLogWarning(string.Format("Unable to remove view with ID {0}, {1}", ID, (_viewsRemoveList.Contains(ID) ? "already being removed." : "instance of view does not exist.")));
      }
    }

    /// <summary>
    /// Removes all of the active views
    /// </summary>
    public override void RemoveAllViews()
    {
      IComparable[] occupiedLayers = GetOccupiedLayers();

      for (int i = 0; i < occupiedLayers.Length; i++)
      {
        RemoveView(_activeViews[occupiedLayers[i]].viewID);
      }

      _greyoutImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Removes all the view except for those on a specified exempt layer
    /// </summary>
    /// <param name="exemptLayers">The layers that you do not want to remove views from</param>
    public override void RemoveAllViews(IComparable[] exemptLayers)
    {
      IComparable[] occupiedLayers = GetOccupiedLayers();

      for (int i = 0; i < occupiedLayers.Length; i++)
      {
        //Only remove the view if its layer does not exist in the exemptLayers array
        if (System.Array.IndexOf<IComparable>(exemptLayers, occupiedLayers[i]) == -1)
        {
          RemoveView(_activeViews[occupiedLayers[i]].viewID);
          UpdateGreyoutPosition(occupiedLayers[i]);
        }
      }
    }

    /// <summary>
    /// Updates the specified View with the data given if the view currently exists in the Canvas.
    /// </summary>
    /// <param name="ID">ID of the View to update.</param>
    /// <param name="data">The object containing the data you wish to pass.</param>
    public override void UpdateView(IComparable ID, object data)
    {
      if (_registeredViews.ContainsKey(ID))
      {
        if (_activeViews.ContainsKey(_registeredViews[ID].layerId))
        {
          IView view = _activeViews[_registeredViews[ID].layerId];

          if (view != null && view.viewID == ID)
          {
            view.UpdateView(data);
          }
          else
          {
            PrintLogWarning("The view you attempted to update is not currently active or null.");
          }
        }
        else
        {
          PrintLogWarning("The view you attempted to update is not present in the ViewCanvas, have you called AddView yet?");
        }
      }
      else
      {
        PrintLogWarning("The view you attempted to update is not registered.");
      }
    }

    /// <summary>
    /// Gets the viewID of the current view on the specified layer. If there is no view on the layer -1 will be returned.
    /// </summary>
    /// <param name="layer">The layer ID to try and retrieve a view ID for.</param>
    /// <returns>The view ID of the view on the specified layer. Returns -1 if no view occupies the layer.</returns>
    public override IComparable GetViewOnLayer(IComparable layer)
    {
      if (_activeViews.ContainsKey(layer) && _activeViews[layer] != null)
      {
        return _activeViews[layer].viewID;
      }
      return -1;
    }
    /// <summary>
    /// Looks at the active views currently on screen. Returns false if the view is not active
    /// </summary>
    /// <param name="viewID">ViewID of the view you want to check</param>
    /// <returns>True if the view is on screen. False if it is not.</returns>
    public override bool IsActiveView(IComparable viewID)
    {
      if (!_registeredViews.ContainsKey(viewID))
      {
        PrintLogWarning(string.Format("No View with ID {0} has been registered.", viewID));
        return false;
      }

      ViewInfo view = _registeredViews[viewID];
      IView viewOnLayer = _activeViews[view.layerId];
      if (viewOnLayer != null && view.viewId == viewOnLayer.viewID)
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Gets an array of the layers that currently have views on them.
    /// </summary>
    /// <returns>An array of occupied layers.</returns>
    public override IComparable[] GetOccupiedLayers()
    {
      List<IComparable> list = new List<IComparable>();
      for (int i = 0; i < _layers.Count; i++)
      {
        if (_activeViews[_layers[i]] != null)
        {
          list.Add(_layers[i]);
        }
      }
      return list.ToArray();
    }

    /// <summary>
    /// Changes the Unity scene to the one specified.
    /// </summary>
    /// <param name="name">Name of the scene to change to.</param>
    /// <param name="loadComplete">An optional loadComplete callback function</param>
    public override void ChangeScene(string name, OnSceneLoaded loadComplete = null)
    {
      if (_loadingScene == true)
      {
        PrintLogWarning("There is already a scene in the process of loading. Attempting to load another scene at the same time may cause unexpected issues!");
      }
      _loadingScene = true;

      if (loadComplete != null)
      {
        _onSceneLoadComplete += loadComplete;
      }

#if LOAD_LOCAL && UNITY_EDITOR
      UnityEditor.EditorApplication.LoadLevelInPlayMode(name);
#else
      Application.LoadLevel(name);
#endif 
    }

    /// <summary>
    /// Changes the Unity scene to the one in the specified asset bundle.
    /// </summary>
    /// <param name="bundlePath">The bundle that the scene belongs to.</param>
    /// <param name="loadComplete">An optional loadComplete callback function</param>
    public override void ChangeSceneBundle(string bundlePath, OnSceneLoaded loadComplete = null)
    {
      if (_loadingScene == true)
      {
        PrintLogWarning("There is already a scene in the process of loading. Attempting to load another scene at the same time may cause unexpected issues!");
      }

      if (_bundleLoadMethod == null)
      {
        PrintLogError("The bundle load method has not been set, bundles cannot be loaded by the View manager.");
        return;
      }

      if (loadComplete != null)
      {
        _onSceneLoadComplete += loadComplete;
      }

      BundleInfo bundleInfo = new BundleInfo(bundlePath);
      _bundleLoadMethod(bundleInfo, ViewLoadedFromBundle);
    }

    /// <summary>
    /// An initialization function that sets the method for loading Asset Bundles
    /// </summary>
    /// <param name="loadMethod">The function for loading Asset Bundles</param>
    public override void SetBundleLoadMethod(BundleLoadMethod loadMethod)
    {
      _bundleLoadMethod = loadMethod;
    }

    /// <summary>
    /// An initialization function that links the canvas that will be used by the view manager
    /// </summary>
    /// <param name="canvas">The Canvas to link</param>
    public void LinkCanvas(Canvas canvas)
    {
      if (_viewCanvas != null)
      {
        PrintLogWarning("The View Canvas was already linked. The new canvas will be linked and the old one will be orphaned.");
      }

      if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
      {
        PrintLogWarning("The View Canvas is not in Overlay mode. This may cause issues with rendering.");
      }

      _viewCanvas = canvas;
      UnityEngine.Object.DontDestroyOnLoad(_viewCanvas);
      CreateLayers();
      CreateGreyoutImage();
    }

    /// <summary>
    /// Assign a name to the specified layer for readability in the Hierarchy
    /// </summary>
    /// <param name="layerId">The ID of the layer to name</param>
    /// <param name="name">THe name to assign to the layer</param>
    public void NameLayer(IComparable layerId, string name)
    {
      if (_layerNames.ContainsKey(layerId))
      {
        Transform transform = _viewCanvas.transform.FindChild(_layerNames[layerId]);
        _layerNames[layerId] = name;
        if (transform != null)
        {
          transform.name = name;
        }
      }
      else
      {
        _layerNames.Add(layerId, name);
      }
    }

    /// <summary>
    /// Declare layers that should show a greyout layer beneath them when opened
    /// </summary>
    /// <param name="overlayLayers">The layers that should have a greyout beneath them</param>
    public void DeclareOverlays(List<IComparable> overlayLayers)
    {
      _overlayLayers = overlayLayers;
      _overlayLayers.Sort();
    }

    /// <summary>
    /// Sets the target alpha of the greyout
    /// </summary>
    /// <param name="alpha">Target alpha</param>
    public void SetGreyoutAlpha(float alpha)
    {
      _greyoutTargetAlpha = alpha;
    }

    /// <summary>
    /// Toggles the view canvas' interactability, enabling or disabling all input-consuming elements.
    /// </summary>
    public override void ToggleCanvasInput(bool enabled)
    {
      if (enabled)
      {
        --_inputDisableCounter;

        // If counter was returned to zero, re-enable interactability
        if (_inputDisableCounter == 0)
        {
          GetCanvasGroup().interactable = enabled;
        }
      }
      else
      {
        // If counter is now non-zero, disable interactability
        if (_inputDisableCounter == 0)
        {
          GetCanvasGroup().interactable = enabled;
        }

        ++_inputDisableCounter;
      }

#if UNITY_EDITOR
      if (_inputDisableCounter < 0)
      {
        throw new System.Exception("View Manager input disable counter was reduced below zero!");
      }
#endif
    }

    /// <summary>
    /// Registers a layer for use in the View Manager. Also created hte layer if it does not already exist.
    /// </summary>
    /// <param name="layerId">ID of hte layer to register</param>
    private void RegisterLayer(IComparable layerId)
    {
      if (_layers.Contains(layerId) == false)
      {
        _layers.Add(layerId);
        _activeViews.Add(layerId, null);

        if (!_layerNames.ContainsKey(layerId))
        {
          _layerNames.Add(layerId, layerId.ToString());
        }

        if (_viewCanvas != null)
        {
          CreateLayers();
        }
      }
    }

    /// <summary>
    /// Creates and sorts all layers to ensure they fall in the proper order within the hierarchy
    /// </summary>
    private void CreateLayers()
    {
      _layers.Sort();
      for (int i = 0; i < _layers.Count; i++)
      {
        Transform transform = _viewCanvas.transform.FindChild(_layerNames[_layers[i]]);

        if (transform != null)
        {
          transform.SetAsLastSibling();
        }
        else
        {
          GameObject gameObject = new GameObject(_layerNames[_layers[i]]);
          RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
          rectTransform.SetParent(_viewCanvas.transform);
          rectTransform.Reset(RectTransformExtensions.AnchorModes.Stretch);
          rectTransform.SetAsLastSibling();
        }
      }
    }

    /// <summary>
    /// Creates a greyout image to place below modals in the ViewCanvas in order to block input and bring focus to the modal
    /// </summary>
    private void CreateGreyoutImage()
    {
      GameObject go = new GameObject();
      UnityEngine.Object.DontDestroyOnLoad(go);
      go.hideFlags = HideFlags.HideInHierarchy;
      go.name = "GreyoutImage";
      go.transform.SetParent(_viewCanvas.transform);
      go.transform.localPosition = Vector3.zero;
      go.transform.localScale = Vector3.one;
      _greyoutImage = go.AddComponent<Image>();
      _greyoutImage.rectTransform.SetSize(new Vector2(_viewCanvas.pixelRect.width, _viewCanvas.pixelRect.height));
      _greyoutImage.rectTransform.SetAnchors(new Vector2(0, 0), new Vector2(1, 1));
      _greyoutImage.color = new Color(0, 0, 0, 0.3f);
      SetGreyoutState(false, true);
    }

    /// <summary>
    /// Creates and transitions in the View
    /// </summary>
    /// <param name="viewInfo">The info required to create the View</param>
    private void CreateAndAddView(ViewInfo viewInfo)
    {
      IView view = null;
      RectTransform rectTransform = (RectTransform)_viewCanvas.transform.FindChild(_layerNames[viewInfo.layerId]);

      if (viewInfo.loadFromBundle == false)
      {
        GameObject loadedView = Resources.Load<GameObject>(viewInfo.viewPath);
        GameObject newView = UnityEngine.Object.Instantiate<GameObject>(loadedView);
        UnityEngine.Object.DontDestroyOnLoad(newView);

        newView.GetComponent<RectTransform>().SetParent(rectTransform, false);
        view = (IView)newView.GetComponent(typeof(IView));

        if (view != null)
        {
          _activeViews[viewInfo.layerId] = view;
          _activeViews[viewInfo.layerId].TransitionIn(delegate () { });

          view.SetViewID(viewInfo.viewId);

          UpdateGreyoutPosition(viewInfo.layerId);
        }
        else
        {
          PrintLogError(string.Format("The view object you attempted to add is not of type IView: {0}", newView.name));
        }
      }
      else
      {
        if (_bundleLoadMethod == null)
        {
          PrintLogError("The bundle load method has not been set, bundles cannot be loaded by the View manager.");
          return;
        }

        _bundleLoadMethod(viewInfo, ViewLoadedFromBundle);
      }
    }

    /// <summary>
    /// A handler for a scene load success event. Will verify that the loaded scene is the one that is expected.
    /// </summary>
    /// <param name="sceneName">Name of hte scene we expected to be loaded.</param>
    private void OnSceneLoadComplete(string sceneName)
    {
#if !UNITY_EDITOR && !LOAD_LOCAL
      if (_loadedScene.ToLower() != sceneName.ToLower())
      {
        throw new System.Exception("Unity Scene's should only be loaded through the View Manager. Use AppHub.viewManager.ChangeScene(string)");
      }
#endif 

      _loadingScene = false;

      if (_onSceneLoadComplete != null)
      {
        _onSceneLoadComplete();
        _onSceneLoadComplete = null;
      }
    }

    /// <summary>
    /// Handles a bundle load complete for the View Manager.
    /// Adds a view to the appropriate layer after being loaded from an asset bundle if the viewObj is not null,
    /// Otherwise attempts to load a scene.
    /// </summary>
    /// <param name="info">The bundle info associated with this load.</param>
    /// <param name="viewObj">The actual view</param>
    private void ViewLoadedFromBundle(BundleInfo info, System.Object viewObj)
    {
      if (viewObj != null)
      {
        ViewInfo viewInfo = info as ViewInfo;
        GameObject viewGO = viewObj as GameObject;

        if (viewGO != null)
        {
          IView view = null;
          RectTransform rectTransform = (RectTransform)_viewCanvas.transform.FindChild(_layerNames[viewInfo.layerId]);

          GameObject newView = UnityEngine.Object.Instantiate<GameObject>(viewGO);
          UnityEngine.Object.DontDestroyOnLoad(newView);

          newView.GetComponent<RectTransform>().SetParent(rectTransform, false);
          view = (IView)newView.GetComponent(typeof(IView));

          if (view != null)
          {
            _activeViews[viewInfo.layerId] = view;
            _activeViews[viewInfo.layerId].TransitionIn(delegate () { });

            view.SetViewID(viewInfo.viewId);
          }
          else
          {
            PrintLogError(string.Format("The view object you attempted to add is not of type IView: {0}", newView.name));
          }
        }
      }
    }

    /// <summary>
    /// Updates the position in the ViewCanvas that the greyout will appear at. This is used to ensure that the greyout is always below the topmost overlay.
    /// </summary>
    /// <param name="updatedLayer">The layer that was updated. We only update the greyout position if an overlay layer was updated</param>
    private void UpdateGreyoutPosition(IComparable updatedLayer)
    {
      //Only update the greyout position if there are registered overlay layers
      if (_overlayLayers != null && _overlayLayers.Contains(updatedLayer))
      {
        //Find the top-most overlay layer and place the greyout image beneath it
        for (int i = _overlayLayers.Count - 1; i >= 0; i--)
        {
          IView view = _activeViews[_overlayLayers[i]];
          if (view != null)
          {
            //set the greyout image as the view's child index
            _greyoutImage.transform.SetSiblingIndex((view as MonoBehaviour).transform.parent.GetSiblingIndex());
            if ((view as MonoBehaviour).transform.parent.GetSiblingIndex() < _greyoutImage.transform.GetSiblingIndex())
            {
              //bump the view down the child hierarchy if the grey image is now overtop of the view. This happens when the grey image travels up the child hierarchy.
              (view as MonoBehaviour).transform.parent.SetSiblingIndex(_greyoutImage.transform.GetSiblingIndex());
            }
            SetGreyoutState(true);
            return;
          }
        }

        SetGreyoutState(false);
      }
    }

    /// <summary>
    /// Enables or disables the greyout along with a quick fade animation.
    /// </summary>
    /// <param name="activeState">The state of the greyout</param>
    /// <param name="skipAnimation">whether to skip the fade animation or not</param>
    private void SetGreyoutState(bool activeState, bool skipAnimation = false)
    {
      if (_greyoutTransition != null)
      {
        CoroutineRunner.StopCoroutine(_greyoutTransition);
      }

      if (skipAnimation == true)
      {
        _greyoutImage.gameObject.SetActive(activeState);
        _greyoutImage.color = new Color(0, 0, 0, activeState ? _greyoutTargetAlpha : 0);
        return;
      }

      if (activeState == true)
      {
        _greyoutImage.gameObject.SetActive(activeState);
        _greyoutTransition = TransitionGreyout(_greyoutTargetAlpha);
      }
      else
      {
        _greyoutTransition = TransitionGreyout(0);
      }

      CoroutineRunner.RunCoroutine(_greyoutTransition);
    }

    /// <summary>
    /// Manages the fading of the greyout
    /// </summary>
    /// <param name="targetAlpha">The target alpha of the greyout</param>
    private System.Collections.IEnumerator TransitionGreyout(float targetAlpha)
    {
      Color _greyoutImageColor = _greyoutImage.color;
      bool isFadingIn = _greyoutImage.color.a < targetAlpha ? true : false;

      if (isFadingIn)
      {
        while (_greyoutImage.color.a < targetAlpha)
        {
          _greyoutImageColor.a += 0.02f;
          _greyoutImage.color = _greyoutImageColor;
          yield return new WaitForSeconds(TRANSITION_TIMESTEP);
        }
      }
      else
      {
        while (_greyoutImage.color.a > targetAlpha)
        {
          _greyoutImageColor.a -= 0.02f;
          _greyoutImage.color = _greyoutImageColor;
          yield return new WaitForSeconds(TRANSITION_TIMESTEP);
        }
      }

      if (_greyoutImage.color.a == 0)
      {
        _greyoutImage.gameObject.SetActive(false);
      }

      _greyoutTransition = null;
    }

    /// <summary>
    /// Gets the view canvas' CanvasGroup component, adding one if none exists.
    /// </summary>
    private CanvasGroup GetCanvasGroup()
    {
      if (_canvasGroup == null)
      {
        _canvasGroup = _viewCanvas.GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
          _canvasGroup = _viewCanvas.gameObject.AddComponent<CanvasGroup>();
        }
      }

      return _canvasGroup;
    }

    #region Logging

    /// <summary>
    /// An initialization command that sets the debug methods for use by the view manager. If these methods are not set, the Manager will not output messages.
    /// </summary>
    /// <param name="log">The log function</param>
    /// <param name="logWarning">The log warning function</param>
    /// <param name="logError">The log error function</param>
    public void SetDebugMethods(ViewManager.LogMethod log, ViewManager.LogMethod logWarning, ViewManager.LogMethod logError)
    {
      _log = log;
      _logWarning = logWarning;
      _logError = logError;
    }

    /// <summary>
    /// Outputs a message
    /// </summary>
    /// <param name="msg">Message to output</param>
    private void PrintLog(string msg)
    {
      if (_log != null)
      {
        _log(msg);
      }
    }

    /// <summary>
    /// Outputs a warning
    /// </summary>
    /// <param name="msg">warning to output</param>
    private void PrintLogWarning(string msg)
    {
      if (_logWarning != null)
      {
        _logWarning(msg);
      }
    }

    /// <summary>
    /// Outputs an error
    /// </summary>
    /// <param name="msg">Error to output</param>
    private void PrintLogError(string msg)
    {
      if (_logError != null)
      {
        _logError(msg);
      }
    }
    #endregion
  }

  /// <summary>
  /// A monobehaviour dedicated to providing insight into Scene load information
  /// </summary>
  /// <remarks>By sharing a .cs file that does not share a name with this class we effectively hide the Monobehaviour from Unity. 
  /// This prevents someone from accidentally stumbling upon the component and trying to add it manually.</remarks>
  public class SceneLoadHandler : MonoBehaviour
  {
    public delegate void SceneLoadCompleteHandler(string sceneName);
    public event SceneLoadCompleteHandler sceneLoadComplete;

    private void OnLevelWasLoaded(int level)
    {
      if (sceneLoadComplete != null)
      {
        sceneLoadComplete(Application.loadedLevelName);
      }
    }
  }
}
