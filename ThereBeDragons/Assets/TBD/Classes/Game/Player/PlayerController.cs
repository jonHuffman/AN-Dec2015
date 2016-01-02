using Core;
using Core.Data;
using Core.Module.EventManager;
using TBD.Events;
using UnityEngine;

namespace TBD
{
  /// <summary>
  /// Controls the Player's avatar in game utilizing the Physics engine to handle movement.
  /// </summary>
  [RequireComponent(typeof(Collider2D))]
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerController : MonoBehaviour, IEventObserver
  {
    /// <summary>
    /// The furthest angle that the avatar will rotate to when falling
    /// </summary>
    private const float MAX_FALL_ANGLE = -90f;

    [SerializeField]
    [Tooltip("The angle to set the character to when input is made.")]
    private float _bounceVisualRotation = 25f;

    /// <summary>
    /// Used to drop the character back to a better X position for playing the game
    /// </summary>
    [SerializeField]
    [Tooltip("The target position of the character once the game starts")]
    private Transform _targetXPosition;

    private TransformData _resetPosition;
    private Rigidbody2D _rigidBody;
    private float _screenTop;
    private float _screenBottom;
    private bool _gameRunning;

    //Used by SmoothDampAngle to smoothly rotate the avatar to its MAX_FALL_ANGLE
    private float _rotationVelocity = 0f;

    //Used to update the avatar's X position when the game starts
    private float _positionVelocity = 0f;

    #region Unity

    void Awake()
    {
      AppHub.eventManager.Register(this);

      _resetPosition = transform.GetData();
      _rigidBody = GetComponent<Rigidbody2D>();

      _rigidBody.isKinematic = true;
      _gameRunning = false;
    }

    void Update()
    {
      //If the character hits the bottom of hte screen, disable physics control
      if (transform.position.y < _screenBottom)
      {
        _rigidBody.isKinematic = true;

        //If the game is still running, fire a game over event
        if (_gameRunning == true)
        {
          AppHub.eventManager.Dispatch(GameEvent.GameOver);
        }
      }
    }

    void LateUpdate()
    {

      //Only update the character if the game has been started and control has been handed back to the physics system
      if (_rigidBody.isKinematic == false)
      {
        if (Input.GetKeyDown(KeyCode.Space))
        {
          //Clear velocity and bounce upwards
          _rigidBody.velocity = Vector2.zero;
          _rigidBody.AddForce(new Vector2(0, 500), ForceMode2D.Force);
          _rotationVelocity = 0f;

          gameObject.transform.Rotate(new Vector3(0, 0, (_bounceVisualRotation - gameObject.transform.eulerAngles.z)));
        }

        float zRot = Mathf.SmoothDampAngle(gameObject.transform.eulerAngles.z, MAX_FALL_ANGLE, ref _rotationVelocity, 1f);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, zRot);

        //If the players X position isn't at our playtime target, update towards it
        if (gameObject.transform.position.x != _targetXPosition.position.x)
        {
          float xPos = Mathf.SmoothDamp(gameObject.transform.position.x, _targetXPosition.position.x, ref _positionVelocity, 0.75f);
          gameObject.transform.position = new Vector3(xPos, gameObject.transform.position.y, gameObject.transform.position.z);
        }
      }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
      //Since in this game any collision causes game over I'm not going to bother with checks
      AppHub.eventManager.Dispatch(GameEvent.GameOver);
    }

    void OnDestroy()
    {
      AppHub.eventManager.Unregister(this);
    }

    void OnDrawGizmos()
    {
      //Debug gizmos to indicate where the character will navigate to along hte X-axis once the game starts.
      if (Application.isPlaying == false && _targetXPosition != null)
      {
        Gizmos.DrawLine(transform.position, _targetXPosition.position);
        Gizmos.DrawWireSphere(_targetXPosition.position, 0.5f);
      }
    }
    #endregion

    #region IEventObserver

    public void OnNotify(System.IComparable gameEvent, object data)
    {
      switch ((GameEvent)gameEvent)
      {
        case GameEvent.StartGame:
          _rigidBody.isKinematic = false;
          _gameRunning = true;
          break;

        case GameEvent.GameOver:
          _gameRunning = false;
          break;

        case GameEvent.Restart:
          transform.position = _resetPosition.position;
          transform.rotation = _resetPosition.rotation;
          transform.localScale = _resetPosition.localScale;
          
          _rigidBody.velocity = Vector2.zero;
          _rigidBody.AddForce(new Vector2(0, 500), ForceMode2D.Force);
          _rotationVelocity = 0f;
          _positionVelocity = 0f;
          break;
      }
    }
    #endregion

    /// <summary>
    /// Uses the game camera to calculate and set the top and bottom values for the camera viewport. Used to determine when the avatar is off screen.
    /// </summary>
    /// <param name="camera">The game camera</param>
    /// <remarks>This exists so that if we decide to add character selection later it just works.</remarks>
    public void SetCameraExtents(Camera camera)
    {
      _screenTop = camera.transform.position.y + camera.orthographicSize;
      _screenBottom = camera.transform.position.y - camera.orthographicSize;
    }
  }
}