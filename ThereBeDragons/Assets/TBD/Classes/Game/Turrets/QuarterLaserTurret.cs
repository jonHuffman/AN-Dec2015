using Core.Utils;
using DG.Tweening;
using TBD.Events;
using UnityEngine;

namespace TBD
{
  public class QuarterLaserTurret : BaseTurret
  {
    private const float GUIDE_START_WIDTH = 0f;
    private const float GUIDE_END_WIDTH = 3f;
    private const float LOCK_ON_SPEED = 1f;
    private const float BLAST_SPEED = 2f;

    [SerializeField, Tooltip("The object containing the pivot that the gun will rotate around.")]
    private Transform _gunHinge;
    [SerializeField]
    private LineRenderer _guide;
    [SerializeField]
    private float _minRotationAngle;
    [SerializeField]
    private float _maxRotationAngle;
    [SerializeField]
    private GameObject _laserBlast;

    private bool _firing = false;
    private float _guideDefaultWidth;
    private int _lockOnTweenID;
    private bool _running;

    #region Unity

    void Awake()
    {
      _guide.enabled = false;
      _running = true;
    }

    protected override void Update()
    {
      if (_running == true)
      {
        if (Vector3.Distance(_player.transform.position, transform.position + new Vector3(_rangeCenter.x, _rangeCenter.y)) <= _range)
        {
          Vector3 vectorToTarget = _player.transform.position - _gunHinge.transform.position;
          float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

          //Correct angle for direction, orientation, and range, while also determining if we can fire
          angle = angle > 0 ? angle : angle + 360;
          angle = _orientation == Orientation.Natural ? angle : 360 - angle;
          if (angle > _minRotationAngle && angle < _maxRotationAngle)
          {
            if (_firing == false)
            {
              _firing = true;
              LockOn();
            }
          }
          else
          {
            _firing = false;
            _guide.enabled = false;
            DOTween.Kill(_lockOnTweenID);
            angle = Mathf.Clamp(angle, _minRotationAngle, _maxRotationAngle);
          }

          Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
          _gunHinge.transform.rotation = q;
        }
      }

      base.Update();
    }
    #endregion

    public override void OnNotify(System.IComparable gameEvent, object data)
    {
      if ((GameEvent)gameEvent == GameEvent.GameOver)
      {
        _running = false;
      }

      base.OnNotify(gameEvent, data);
    }

    /// <summary>
    /// Starts the "lock-on" step in the firing procedure. This exists to warn players that they are in range.
    /// </summary>
    private void LockOn()
    {
      float guideWidth = GUIDE_END_WIDTH;

      _guide.SetWidth(GUIDE_START_WIDTH, GUIDE_END_WIDTH);
      _guide.enabled = true;
      _lockOnTweenID = Helpers.UniqueID;

      DOTween.To(() => guideWidth, w => guideWidth = w, 0f, LOCK_ON_SPEED)
        .SetId(_lockOnTweenID)
        .OnUpdate(() => _guide.SetWidth(GUIDE_START_WIDTH, guideWidth))
        .OnComplete(FireTurret);
    }

    /// <summary>
    /// Creates an instance of the laser and launches it in the direction of the player
    /// </summary>
    private void FireTurret()
    {
      GameObject blast = GameObject.Instantiate(_laserBlast);
      blast.transform.position = new Vector3(_gunHinge.transform.position.x, _gunHinge.transform.position.y, blast.transform.position.z);
      blast.transform.rotation = _gunHinge.transform.rotation;

      //Multiply by 10 to guarantee it leave the screen
      Vector2 target = (_player.transform.position - _gunHinge.transform.position) * 10;
      blast.transform.DOMove(new Vector3(target.x, target.y, blast.transform.position.z), BLAST_SPEED)
        .OnComplete(() =>
          {
            GameObject.Destroy(blast);
          });

      _firing = false;
    }
  }
}