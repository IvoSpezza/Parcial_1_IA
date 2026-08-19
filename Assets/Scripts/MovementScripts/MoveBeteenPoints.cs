using UnityEngine;

public class MoveBeteenPoints : Agent
{

    [SerializeField] private float _maxSpeed = 6f;

    [SerializeField] private Transform[] _points;

    private int _actualPoint = 0;
    
    void Update()
    {
        SeekPoint();
        transform.position += _actualVelocity;
        transform.forward = _actualVelocity;
    }

    public void SeekPoint()
    {
        Vector3 direction = _points[_actualPoint].position - transform.position ;

        float distance = direction.magnitude;

        if (distance <= 0.1f)
        {
            if (_actualPoint == _points.Length -1)
            {
                _actualPoint = 0;
            }
            else
            {
                _actualPoint++;
            }
            direction = _points[_actualPoint].position - transform.position;
        }

        direction.Normalize();

        direction *= _maxSpeed;

        Vector3 Steering = direction - _actualVelocity;

        _actualVelocity = Vector3.ClampMagnitude(Steering, _maxSpeed * Time.deltaTime);
    }

}
