using UnityEngine;

namespace TBD
{
  public class MissileTurret : BaseTurret
  {
    [SerializeField]
    private Missile _missile;

    #region Unity
    
    protected override void Update()
    {
      if (_running == true && _missile.launched == false)
      {
        if (Vector3.Distance(_player.transform.position, transform.position + new Vector3(_rangeCenter.x, _rangeCenter.y)) <= _range)
        {
          Vector3 vectorToTarget = _player.transform.position - _missile.transform.position;
          float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

          //Correct angle for direction, orientation, and range, while also determining if we can fire
          angle = angle > 0 ? angle : angle + 360;
          angle = _orientation == Orientation.Natural ? angle : 360 - angle;

          if (angle > 0 && angle < 180)
          {
            _missile.LockOnAndLaunch(_orientation);
          }
          else
          {
            //_missile.AbortLaunch();
          }
        }
        else
        {
          //_missile.AbortLaunch();
        }
      }

      base.Update();
    }
    #endregion

    public override void Init(float minSpawnY, float maxSpawnY, float minCamFrameX, float maxCamFrameX, PlayerController player)
    {
      _missile.SetPlayer(player);
      base.Init(minSpawnY, maxSpawnY, minCamFrameX, maxCamFrameX, player);
    }
  }
}
