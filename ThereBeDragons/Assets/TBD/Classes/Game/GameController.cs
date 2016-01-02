using Core;
using Core.Module.EventManager;
using System.Collections;
using System.Collections.Generic;
using TBD.Events;
using TBD.Views;
using UnityEngine;

namespace TBD
{
  public class GameController : MonoBehaviour, IEventObserver
  {
    private const float START_GAME_SPEED = 1f;
    private static float GAME_SPEED = START_GAME_SPEED;

    [SerializeField]
    private float _gameStartDelay = 2f;
    [SerializeField, Tooltip("The rate that the game's speed should increase by each second"), Range(1, float.MaxValue)]
    private float _gameSpeedAcceleration = 1.01f;
    private float _maxGameSpeed = 3f;
    [SerializeField]
    private Camera _gameCamera;
    [SerializeField]
    private PlayerController _playerAvatar;
    [Tooltip("List of references to the Turret prefabs that can spawn in game.")]
    [SerializeField]
    private List<BaseTurret> _turrets;

    //The X extents of the camera's frame
    private float _minCamFrameX;
    private float _maxCamFrameX;

    #region Turret Spawn Variables

    private float _maxTurretSpawnY;
    private float _minTurretSpawnY;
    /// <summary>
    /// The value at which we should force the next spawned turret to be on the opposit side of the screen.
    /// </summary>
    /// <remarks>
    /// Without this it is possible to have a run of turrets that are particularly easy to avoid. The game should involve a lot of maneuvering.
    /// </remarks>
    private float _forceFlipThreshold;
    private bool _flipNextTurret = false;
    private BaseTurret _prevTurret;
    #endregion

    private float _startTime;
    private int _attempt = 0;
    private bool _gameRunning = false;
    private UIData _uiData;

    private IEnumerator _turretSpawner;

    #region Properties
    public static float Speed
    {
      get { return GAME_SPEED; }
    }
    #endregion

    #region Unity
    void Awake()
    {
      AppHub.eventManager.Register(this);

      float cameraWidth = (_gameCamera.orthographicSize * 2) * _gameCamera.aspect;
      _minCamFrameX = _gameCamera.transform.position.x - (cameraWidth / 2);
      _maxCamFrameX = _gameCamera.transform.position.x + (cameraWidth / 2);

      _maxTurretSpawnY = _gameCamera.transform.position.y;
      _minTurretSpawnY = _gameCamera.transform.position.y - _gameCamera.orthographicSize;
      //If the turret spawns in the top third of the spawn range the next turret will be spawned on the opposite side of the screen
      _forceFlipThreshold = (Mathf.Abs(_maxTurretSpawnY - (Mathf.Abs(_maxTurretSpawnY - _minTurretSpawnY) / 3)));

      _playerAvatar.SetCameraExtents(_gameCamera);
    }

    void Update()
    {
      if (_gameRunning == true)
      {
        if (GAME_SPEED < _maxGameSpeed)
        {
          GAME_SPEED += ((GAME_SPEED * _gameSpeedAcceleration) - GAME_SPEED) * Time.deltaTime;
          GAME_SPEED = GAME_SPEED < _maxGameSpeed ? GAME_SPEED : _maxGameSpeed;
        }

        _uiData.time = Mathf.FloorToInt(Time.time - _startTime);
        _uiData.attempt = _attempt;
        AppHub.viewManager.UpdateView(View.UI, _uiData);
      }
    }

    void OnDestroy()
    {
      AppHub.eventManager.Unregister(this);
    }
    #endregion

    #region IEventObserver

    public void OnNotify(System.IComparable gameEvent, object data)
    {
      switch ((GameEvent)gameEvent)
      {
        case GameEvent.StartGame:
          _attempt++;
          _startTime = Time.time;
          _gameRunning = true;
          GAME_SPEED = START_GAME_SPEED;

          if (_turrets.Count != 0)
          {
            _turretSpawner = TurretSpawner();
            StartCoroutine(_turretSpawner);
          }
          break;

        case GameEvent.GameOver:
          StopCoroutine(_turretSpawner);
          GAME_SPEED = 0f;
          _gameRunning = false;
          AppHub.viewManager.AddView(View.GameOver, _uiData);
          break;
      }
    }
    #endregion

    /// <summary>
    /// Spawns turret every X seconds where X is the start delay multiplied by the game speed
    /// </summary>
    private IEnumerator TurretSpawner()
    {
      while (true)
      {
        yield return new WaitForSeconds(_gameStartDelay / GAME_SPEED);
        SpawnTurret();
      }
    }

    /// <summary>
    /// Creates the new turrent and determines whether or not it should be flipped, anchoring it to the top or bottom of the screen
    /// </summary>
    private void SpawnTurret()
    {
      BaseTurret turret = GameObject.Instantiate<BaseTurret>(_turrets[Random.Range(0, _turrets.Count - 1)]);
      turret.Init(_minTurretSpawnY, _maxTurretSpawnY, _minCamFrameX, _maxCamFrameX, _playerAvatar);

      if ((_flipNextTurret == true && _prevTurret != null && _prevTurret.orientation == BaseTurret.Orientation.Natural))
      {
        turret.FlipTurret();
        _flipNextTurret = false;
      }
      else if (Random.Range(0, 1) == 1)
      {
        turret.FlipTurret();
      }

      if (Mathf.Abs(turret.transform.position.y) < _forceFlipThreshold)
      {
        _flipNextTurret = true;
      }

      _prevTurret = turret;
    }
  }
}