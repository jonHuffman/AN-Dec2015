using Core;
using Core.Module.EventManager;
using TBD.Events;
using UnityEngine;

namespace TBD
{
  [RequireComponent(typeof(Collider2D))]
  public class BaseTurret : MonoBehaviour, IEventObserver
  {
    //Distance per second
    private const float MOVE_SPEED = 2f;

    /// <summary>
    /// The orientation of the turret
    /// </summary>
    public enum Orientation
    {
      Natural,
      Inverse
    }

    [SerializeField]
    protected float _range = 1f;
    [SerializeField]
    protected Vector2 _rangeCenter = Vector2.zero;

    protected PlayerController _player;
    //The point along the X axis that the turret should destroy itself at
    private float _turretKillPoint;
    private Bounds _colliderBounds;
    protected Orientation _orientation = Orientation.Natural;

    #region Properties

    /// <summary>
    /// The bounds of all colliders on the turrent
    /// </summary>
    public Bounds colliderBounds
    {
      get
      {
        //Bounds is not nullable so we will check on the size
        if (_colliderBounds.size == Vector3.zero)
        {
          Collider2D[] colliders = GetComponents<Collider2D>();
          _colliderBounds = colliders[0].bounds;

          for (int i = 1; i < colliders.Length; i++)
          {
            _colliderBounds.Encapsulate(colliders[i].bounds);
          }
        }

        return _colliderBounds;
      }
    }

    /// <summary>
    /// The orientation of the turret
    /// </summary>
    public Orientation orientation
    {
      get
      {
        return _orientation;
      }
    }
    #endregion

    #region Unity

    protected virtual void Update()
    {
      if (transform.position.x < _turretKillPoint)
      {
        Destroy(gameObject);
      }
      else
      {
        Vector3 pos = transform.position;
        pos.x -= MOVE_SPEED * GameController.Speed * Time.deltaTime;
        transform.position = pos;
      }
    }

    void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(new Vector3(_rangeCenter.x, _rangeCenter.y) + transform.position, _range);
    }

    void OnDestroy()
    {
      //This will be invoked twice when a restart notification is recieved. THe event manager will handle this.
      AppHub.eventManager.Unregister(this);
    }
    #endregion

    #region IEVentObserver
    
    public virtual void OnNotify(System.IComparable gameEvent, object data)
    {
      if((GameEvent)gameEvent == GameEvent.Restart)
      {
        //Unregister immediately so that other events do get recieved by this during the destroy process
        AppHub.eventManager.Unregister(this);
        Destroy(gameObject);
      }
    }
    #endregion

    /// <summary>
    /// Initializes the turret, setting its position and the X value at which the turret should clean itself up
    /// </summary>
    /// <param name="minCamFrameX">The camera frame's minimum X</param>
    public virtual void Init(float minSpawnY, float maxSpawnY, float minCamFrameX, float maxCamFrameX, PlayerController player)
    {
      AppHub.eventManager.Register(this);

      _player = player;
      _turretKillPoint = minCamFrameX - colliderBounds.extents.x;

      //Position turret
      float turretY = Random.Range(minSpawnY, maxSpawnY);
      transform.position = new Vector3(maxCamFrameX + colliderBounds.extents.x, turretY, transform.position.z);
    }

    /// <summary>
    /// Flips a turrets orientation
    /// </summary>
    public void FlipTurret()
    {
      transform.position = new Vector3(transform.position.x, transform.position.y * -1, transform.position.z);
      transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
      _rangeCenter.y = _rangeCenter.y * - 1;

      _orientation = Orientation.Inverse;
    }
  }
}