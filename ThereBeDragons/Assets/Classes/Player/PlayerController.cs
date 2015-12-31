using Core;
using Core.Module.EventManager;
using TBD.Events;
using UnityEngine;

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

  [SerializeField]
  [Tooltip("The X position of the character once the game starts")]
  private Transform _targetX;

  private Collider2D _collider;
  private Rigidbody2D _rigidBody;

  //Used by SmoothDampAngle to smoothly rotate the avatar to its MAX_FALL_ANGLE
  private float _rotationVelocity = 0f;

  //Used to update the avatar's X position when the game starts
  private float _positionVelocity = 0f;

  void Awake()
  {
    AppHub.eventManager.Register(this);

    _collider = GetComponent<Collider2D>();
    _rigidBody = GetComponent<Rigidbody2D>();

    _rigidBody.isKinematic = true;
  }

  void Update()
  {
    //Only update the character if the game has been started
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
      if(gameObject.transform.position.x != _targetX.position.x)
      {
        float xPos = Mathf.SmoothDamp(gameObject.transform.position.x, _targetX.position.x, ref _positionVelocity, 0.75f);
        gameObject.transform.position = new Vector3(xPos, gameObject.transform.position.y, gameObject.transform.position.z);
      }
    }
  }

  void OnDestroy()
  {
    AppHub.eventManager.Unregister(this);
  }

  public void OnNotify(System.IComparable gameEvent, object data)
  {
    if ((GameEvent)gameEvent == GameEvent.StartGame)
    {
      _rigidBody.isKinematic = false;
    }
  }

  void OnDrawGizmos()
  {
    //Debug gizmos to indicate where the character will navigate to along hte X-axis once the game starts.
    if (Application.isPlaying == false && _targetX != null)
    {
      Gizmos.DrawLine(transform.position, _targetX.position);
      Gizmos.DrawWireSphere(_targetX.position, 0.5f);
    }
  }
}
