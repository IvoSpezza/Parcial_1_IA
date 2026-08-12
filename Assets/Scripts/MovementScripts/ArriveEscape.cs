using UnityEngine;

public class ArriveEscape : MonoBehaviour
{
    //Temp
    [SerializeField] private Transform _objetive;

    [SerializeField] private float _maxSpeed = 5;
    [SerializeField] private float _steeringSpeed = 3;
    [SerializeField] private float _maxDistance = 10;
    [SerializeField] private float _minDistance = 0.1f;

    [SerializeField] private bool _arriveOrScape= true;

    private Vector3 actualVelocity;

    void Update()
    {
        if (_arriveOrScape)
        {
            Arrive();
            
        }
        else
        {
            Escape();
        }
        transform.position += actualVelocity * Time.deltaTime;
        transform.forward = actualVelocity;
    }


    public void Arrive()
    {
        Vector3 direction = _objetive.position - transform.position;
        float distance = direction.magnitude;

        if(distance < _minDistance)
        {
            actualVelocity = Vector3.zero;
            return;
        }

        float slowSpeed = _maxSpeed * (distance / _maxDistance);

        float desiredSpeed = Mathf.Min(slowSpeed, _maxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed;

        Vector3 Steering = desired - actualVelocity;

        Steering = Vector3.ClampMagnitude(Steering, _steeringSpeed * Time.deltaTime);

        actualVelocity += Steering;
    }



    public void Escape()
    {
        Vector3 direction = _objetive.position - transform.position;
        float distance = direction.magnitude;

        if (distance > _maxDistance)
        {
            actualVelocity = Vector3.zero;
            return;
        }
        Debug.Log(distance);
        float slowSpeed = _maxSpeed * (_minDistance / distance);

        float desiredSpeed = Mathf.Min(slowSpeed, _maxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed * -1;

        Vector3 Steering = desired - actualVelocity;

        Steering = Vector3.ClampMagnitude(Steering, _steeringSpeed * Time.deltaTime);

        actualVelocity += Steering;
    }
}
