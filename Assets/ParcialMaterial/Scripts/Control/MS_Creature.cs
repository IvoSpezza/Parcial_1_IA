using UnityEngine;

public class MS_Creature : MonoBehaviour
{
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _steering;

    public Vector3 _velocity { get; protected set; }


    public Vector3 CalculateDirection(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        return direction;
    }

    public Vector3 CalculateSteering(Vector3 desired)
    {        
        desired *= _maxSpeed;

        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, _steering * Time.deltaTime);
    }

    public Vector3 CalculateSteering(Vector3 desired, float otherSpeed)
    {
        desired *= otherSpeed;

        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, _steering * Time.deltaTime);
    }

    public Vector3 CalculateFuture(MS_Creature target)
    {
        Vector3 direction = target.transform.position - transform.position;

        float distance = direction.magnitude;

        float prediction = distance / (_maxSpeed + target._velocity.magnitude);

        return target.transform.position + target._velocity * prediction;

    }

    public Vector3 Arrive(Vector3 target, float minDistance, float maxDistance)
    {
        Vector3 direction = target - transform.position;

        float distance = direction.magnitude;

        if (distance <= minDistance)
        {
            return Vector3.zero;
        }

        float desiredSpeed = _maxSpeed * Mathf.Clamp01(distance / maxDistance);

        Vector3 desired = direction.normalized * desiredSpeed;

        Vector3 Steering = CalculateSteering(desired);

        return Steering;
    }

    public void AplyVelocity(Vector3 velocity)
    {
        velocity.y = 0;
        _velocity += velocity;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);
    }

}
