using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MoveBeteenPoints : Agent
{

    [SerializeField] private PatrolStyle _patrolStyle;

    [SerializeField] private float _maxSpeed = 6f;

    [SerializeField] private List<Transform> _points;

    [SerializeField] private float _wayPointRange = 0.01f;

    private int _actualPoint = 0;

    private int _direction = 1;

    private bool _canCheck = true;
    void Update()
    {
        switch (_patrolStyle){
            case PatrolStyle.PingPong:
                PingPongPatrol();
                break;
            case PatrolStyle.Loop:
                PatrolLoop();
                break;
        }
        PatrolLoop();
        transform.position += _actualVelocity;
        transform.forward = _actualVelocity;
    }

    private void PatrolLoop()
    {
        if (Vector3.Distance(_points[_actualPoint].position, transform.position) <= _wayPointRange)
        {
            if (_actualPoint == _points.Count - 1) _actualPoint = 0;
            else _actualPoint++;

        }

        Vector3 direction = (_points[_actualPoint].position - transform.position).normalized;

        direction *= _maxSpeed;

        Vector3 Steering = direction - _actualVelocity;

        _actualVelocity = Vector3.ClampMagnitude(Steering, _maxSpeed * Time.deltaTime);
    }

    private void PingPongPatrol()
    {
        float distance = Vector3.Distance(_points[_actualPoint].position, transform.position);
        if ( distance <= _wayPointRange && _canCheck)
        {

            if (_actualPoint == _points.Count - 1 || (_direction < 0 && _actualPoint == 0))
            {
                _direction *= -1; 
                _actualPoint += _direction;
            } 
            else _actualPoint += _direction;
            _canCheck = false;

            Debug.Log(_actualPoint);

        } else if (distance >= _wayPointRange * 5)
        {
            _canCheck = true;
        }

            Vector3 direction = (_points[_actualPoint].position - transform.position).normalized;

        direction *= _maxSpeed;

        Vector3 Steering = direction - _actualVelocity;

        _actualVelocity = Vector3.ClampMagnitude(Steering, _maxSpeed * Time.deltaTime);
    }
}

public enum PatrolStyle
{
    Loop,PingPong
}
