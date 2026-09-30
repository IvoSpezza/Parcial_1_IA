using UnityEngine;

public class MS_Creature : MonoBehaviour
{
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _steering;

    [SerializeField] protected float _avoidDistance;
    [SerializeField] protected float _avoidAngle;
    [SerializeField, Range(0f,100f)] protected float _avoidWeight = 1;

    public Vector3 _velocity { get; protected set; }
    public float AvoidDistance => _avoidDistance;
    public float AvoidAngle => _avoidAngle;

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
    //permite calcular el steering pero con una velocidad distinta a la base de la creatura
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

    public void Arrive(Vector3 target, float minDistance, float maxDistance)
    {
        Vector3 direction = target - transform.position;

        float distance = direction.magnitude;

        if (distance <= minDistance)
        {
            _velocity = Vector3.zero;
            return ;
        }

        float desiredSpeed = _maxSpeed * Mathf.Clamp01(distance / maxDistance);

        Vector3 desired = direction.normalized * desiredSpeed; 

        Vector3 Steering = CalculateSteering(desired);
        
        AplyVelocity(Steering);
    }

    protected void AvoidObstacles(float eyeHeigt, LayerMask obstacleLayer)
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeigt;

        Vector3 avoidance = Whiskers(origin, transform.forward,1f,obstacleLayer);

        avoidance += Whiskers(origin, Quaternion.Euler(0, _avoidAngle, 0) * transform.forward, 0.5f, obstacleLayer);
        avoidance += Whiskers(origin, Quaternion.Euler(0, -_avoidAngle, 0) * transform.forward, 0.5f, obstacleLayer);
        Debug.Log(avoidance);
        AplyVelocity(Vector3.ClampMagnitude(avoidance,_avoidWeight * Time.deltaTime));
    }

    private Vector3 Whiskers(Vector3 origin, Vector3 direcction, float weight, LayerMask obstacleLayer)
    {
        if(!Physics.Raycast(origin,direcction,out RaycastHit hit, _avoidDistance,obstacleLayer)) return Vector3.zero;
        if (hit.transform.IsChildOf(transform)) return Vector3.zero;

        float proximity = 1f - (hit.distance / _avoidDistance);
        Vector3 away = Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized;
        Vector3 lateral = Vector3.ProjectOnPlane(away, transform.forward);

        if(lateral.sqrMagnitude < 0.001f)
        {
            lateral = transform.right;
        }
        return lateral.normalized * proximity * weight;
    }

    public void AplyVelocity(Vector3 velocity)
    {
        velocity.y = 0;
        _velocity += velocity;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);
    }        

}
