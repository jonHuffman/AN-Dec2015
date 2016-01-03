using Core;
using Core.Module.EventManager;
using Core.Module.SoundManagerSystem;
using Core.Module.ViewManagerSystem;
using DG.Tweening;
using TBD.Views;
using UnityEngine;

namespace TBD
{
  /// <summary>
  /// Main entry point for the game. Initializes core systems and then opens the Game scene.
  /// </summary>
  public class GameInitializer : MonoBehaviour
  {
    [SerializeField]
    private Canvas _viewCanvas;

    void Awake()
    {
      DOTween.Init(false, false, LogBehaviour.Default).SetCapacity(100, 20);

      InitializeViewManager();
      InitializeEventManager();
      InitializeSoundManager();

      StartGame();
    }

    #region ViewManager
    /// <summary>
    /// Initializes the ViewManager and binds the necessary things for it to run.
    /// </summary>
    private void InitializeViewManager()
    {
      ViewManager viewManager = new ViewManager();

      //Link the canvas that will be used for UI Views in this game
      viewManager.LinkCanvas(_viewCanvas);

      //Set the methods that the View Manager will use for logging
      viewManager.SetDebugMethods(Debug.Log, Debug.LogWarning, Debug.LogError);

      //Link the view manager to our service provider, making it accessible throughout the game
      AppHub.SetViewManager(viewManager);

      NameLayers();
      RegisterViews();
    }

    /// <summary>
    /// Names the layers that will be used by the ViewManager so that they are easy to identify in the editor
    /// </summary>
    private void NameLayers()
    {
      //The base View Manager does not support naming layers so we have to cast it back to the implemented version
      ViewManager viewManager = (AppHub.viewManager as ViewManager);

      viewManager.NameLayer(Layer.Main, "Main");
      viewManager.NameLayer(Layer.Modal, "Modal");
      viewManager.NameLayer(Layer.PersistentUI, "Persistent UI");
    }

    /// <summary>
    /// Registers the Views so that they can be used in the game
    /// </summary>
    private void RegisterViews()
    {
      AppHub.viewManager.RegisterView(View.Start, Layer.Main, "Views/StartView");
      AppHub.viewManager.RegisterView(View.UI, Layer.Main, "Views/UIView");
      AppHub.viewManager.RegisterView(View.GameOver, Layer.Modal, "Views/GameOverDialog");
      AppHub.viewManager.RegisterView(View.PersistentUI, Layer.PersistentUI, "Views/PersistentUIView");
      AppHub.viewManager.RegisterView(View.Credits, Layer.Modal, "Views/CreditsDialog");
    }
    #endregion

    #region EventManager

    /// <summary>
    /// Initializes the EventManager
    /// </summary>
    private void InitializeEventManager()
    {
      EventManager eventManager = new EventManager();
      eventManager.SetDebugMethods(Debug.Log, Debug.LogWarning, Debug.LogError);
      AppHub.SetEventManager(eventManager);
    }
    #endregion

    #region SoundManager

    private void InitializeSoundManager()
    {
      BaseSoundManager soundManager = new GameObject().AddComponent<SoundManager>();
      (soundManager as SoundManager).SetDebugMethods(Debug.Log, Debug.LogWarning, Debug.LogError);

      soundManager.CreateLayer(SoundLayers.Music, false, 0.5f);
      soundManager.CreateLayer(SoundLayers.SFX, true);

      soundManager.Initialize();

      AppHub.SetSoundManager(soundManager);
    }
    #endregion

    /// <summary>
    /// Start the game
    /// </summary>
    private void StartGame()
    {
      AppHub.viewManager.AddView(View.PersistentUI);
      AppHub.viewManager.AddView(View.Start);
      
      AppHub.viewManager.ChangeScene(Scenes.Game);
    }
  }
}