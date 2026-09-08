using UnityEngine;

public abstract class CreatureState
{
    protected float _maxSpeed;
    protected Vector3 _velocity;
    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }

    public Vector3 CalculateSteering(Vector3 desired, float steeringFactor)
    {
        desired *= _maxSpeed;

        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, steeringFactor * Time.deltaTime);
    }
}
