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

    
}
