using Core;
using Core.Module.EventManager;
using System.Collections;
using TBD.Events;
using UnityEngine;

namespace TBD
{
  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(Collider2D))]
  public class Missile : MonoBehaviour, IEventObserver
  {
    private const float ACCELERATION_RATE = 0.3f;
    private const float LAUNCH_DELAY = 0.5f;
    private const float SEEK_TIME = 2f;

    [SerializeField]
    private Sprite _on;
    [SerializeField]
    private Sprite _off;
    [SerializeField]
    private ParticleSystem _explosion;

    private PlayerController _player;
    private BaseTurret.Orientation _turretOrientation;
    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private bool _launched;
    private IEnumerator _blink;
    private Vector2 _velocity = Vector2.zero;
    private float _timeAtLaunch;

    #region properties

    public bool launched
    {
      get { return _launched; }
    }
    #endregion

    #region Unity

    void Awake()
    {
      _collider = GetComponent<Collider2D>();
      _renderer = GetComponent<SpriteRenderer>();
      _renderer.sprite = _off;

      _launched = false;
    }

    void Update()
    {
      //Seek for the first SEEK_TIME seconds
      if (_launched == true)
      {
        if ((Time.time - _timeAtLaunch) < SEEK_TIME)
        {
          Vector2 acceleration = _player.transform.position - transform.position;

          //rotate
          float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
          Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

          //We really only need to set the X
          if (_turretOrientation == BaseTurret.Orientation.Inverse)
          {
            transform.localScale = new Vector3(1, 1, 1);
          }

          acceleration.x = acceleration.x > 0 ? ACCELERATION_RATE : -ACCELERATION_RATE;
          acceleration.y = acceleration.y > 0 ? ACCELERATION_RATE : -ACCELERATION_RATE;

          //Update velocity
          _velocity += (acceleration * ACCELERATION_RATE) * Time.deltaTime;

          transform.rotation = q;
          transform.position = transform.position + new Vector3(_velocity.x, _velocity.y, 0);
        }
        else
        {
          StartCoroutine(Explode());
        }
      }

    }

    void OnTriggerEnter2D(Collider2D obj)
    {
      if (obj.gameObject == _player.gameObject)
      {
        StartCoroutine(Explode());
      }
    }
    #endregion

    #region IEventObserver

    public virtual void OnNotify(System.IComparable gameEvent, object data)
    {
      if ((GameEvent)gameEvent == GameEvent.GameOver || (GameEvent)gameEvent == GameEvent.Restart)
      {
        AppHub.eventManager.Unregister(this);
        Destroy(gameObject);
      }
    }
    #endregion

    /// <summary>
    /// Sets teh player so that the missile can see towards it
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(PlayerController player)
    {
      _player = player;
    }

    /// <summary>
    /// Begins the launch process for the missile
    /// </summary>
    public void LockOnAndLaunch(BaseTurret.Orientation turretOrientation)
    {
      _turretOrientation = turretOrientation;

      if (_blink == null)
      {
        _blink = LockOn();
        StartCoroutine(_blink);
      }
    }

    /// <summary>
    /// Aborts a missile launch
    /// </summary>
    public void AbortLaunch()
    {
      if (_blink != null)
      {
        StopCoroutine(_blink);
        _blink = null;
      }
    }

    /// <summary>
    /// Updates the visual of the rocket and delays the actual launch
    /// </summary>
    private IEnumerator LockOn()
    {
      _renderer.sprite = _on;
      yield return new WaitForSeconds(LAUNCH_DELAY);
      Launch();
    }

    /// <summary>
    /// Handles the destruction animation of the missile
    /// </summary>
    private IEnumerator Explode()
    {
      _explosion.Play();
      _renderer.enabled = false;
      _collider.enabled = false;
      yield return new WaitForSeconds(_explosion.startLifetime);

      AppHub.eventManager.Unregister(this);
      GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// Actually launches the missile causing it to seek towards the player
    /// </summary>
    private void Launch()
    {
      _launched = true;
      _timeAtLaunch = Time.time;

      transform.SetParent(null);

      //Subscribe to event manager now that the missile is an independant object
      AppHub.eventManager.Register(this);
    }
  }
}