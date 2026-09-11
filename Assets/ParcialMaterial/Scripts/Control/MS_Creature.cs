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

    public void AplyVelocity(Vector3 velocity)
    {
        velocity.y = 0;
        _velocity += velocity;
    }

}
