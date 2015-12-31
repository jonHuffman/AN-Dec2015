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
  private Collider2D _collider;
  private Rigidbody2D _rigidBody;

  void Awake()
  {
    AppHub.eventManager.Register(this);

    _collider = GetComponent<Collider2D>();
    _rigidBody = GetComponent<Rigidbody2D>();

    _rigidBody.isKinematic = true;
  }

  void Update()
  {
    //Clear velocity and bounce upwards
    if (Input.GetKeyDown(KeyCode.Space))
    {
      _rigidBody.velocity = Vector2.zero;
      _rigidBody.angularVelocity = 0f;
      _rigidBody.AddForce(new Vector2(0, 500), ForceMode2D.Force);
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
}
