using System.Collections.Generic;
using UnityEngine;

public class S_Patrol : CreatureState
{
    private int _orientation;
    private PatrolData _data;
    private MS_Hunter _me;
    private int _actualPoint;

    private PatrolMetod _actualMetod;


    public S_Patrol(PatrolData data, MS_Hunter me)
    {
        _data = data;
        _me = me;
        _actualPoint = _data._pathPoints.Count / 2;

    }

    public override void Enter()
    {
        _orientation = Random.Range(0, 2);
        _actualMetod = (PatrolMetod)_orientation;
        _orientation = _orientation * 2 - 1;
        Debug.Log(_actualMetod);
        
    }


    public override void Update()
    {
        Patrol();
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;
    }

    public override void Exit()
    {

    }

    public void Patrol()
    {
        float distance = Vector3.Distance(_me.transform.position, Desired());
        if (distance <= _data._minRangeToChange)
        {
            
            if(_actualMetod == PatrolMetod.Loop)
            {
                Loop();
            } else
            {
                PingPong();
            }


                _actualPoint += _orientation;
        }
        Seek(Desired());
    }

    private void Loop()
    {
        if (_actualPoint == 0 && _orientation < 0)
        {
            _actualPoint = _data._pathPoints.Count - 1;
        } else if (_actualPoint == _data._pathPoints.Count - 1 && _orientation > 0)
        {
            _actualPoint = 0;
        }
    }
    private void PingPong()
    {
        if ((_actualPoint == 0 && _orientation < 0) || (_actualPoint == _data._pathPoints.Count - 1 && _orientation > 0))
        {
            _orientation *= -1;
        }
    }


    public void Seek(Vector3 target)
    {
        Vector3 desired = _me.CalculateDirection(target);
        _me.AplyVelocity(_me.CalculateSteering(desired,_data._patrolSpeed));
    }

    //Para entender mejor
    private Vector3 Desired() => _data._pathPoints[_actualPoint].position;


}

[System.Serializable]
public class PatrolData
{
    public List<Transform> _pathPoints;
    public float _minRangeToChange;
    public float _patrolSpeed;
}

public enum PatrolMetod
{
    PingPong,
    Loop
}