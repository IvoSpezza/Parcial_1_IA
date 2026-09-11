using System.Collections.Generic;
using UnityEngine;

public class S_Patrol : CreatureState
{
    private int _orientation;
    private PatrolData _data;
    private MS_Hunter _me;
    private int _actualPoint;

    private bool _loopCompleted;
    private PatrolMetod _actualMetod;

    private float _actualTime;
    private float _tba;
    private bool _iTriedAtack;
    public event System.Action CanAtack;

    public S_Patrol(PatrolData data, MS_Hunter me, float tba)
    {
        _data = data;
        _me = me;
        _tba = tba;
        _actualPoint =Random.Range(0,_data._pathPoints.Count);

    }

    public override void Enter()
    {
        _iTriedAtack = false;
        _actualTime = 0;
        _loopCompleted = false;
        _orientation = Random.Range(0, 2);
        _orientation = Random.Range(0, 2);
        _actualMetod = (PatrolMetod)_orientation;
        _orientation = _orientation * 2 - 1;          
    }


    public override void Update()
    {
        Patrol();
        _actualTime += Time.deltaTime;
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;
        if(_actualTime >= _tba && !_iTriedAtack)
        {
            CanAtack?.Invoke();
            _iTriedAtack = true;
        }
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

            if (!_loopCompleted)
            {
                _actualPoint += _orientation;                
            } else
            {
                _loopCompleted = false;
            }
                
        }
        Seek(Desired());
    }

    private void Loop()
    {
        if (_actualPoint == 0 && _orientation < 0)
        {
            _actualPoint = _data._pathPoints.Count - 1;
            _loopCompleted = true;

        } else if (_actualPoint == _data._pathPoints.Count - 1 && _orientation > 0)
        {
            _actualPoint = 0;
            _loopCompleted = true;
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