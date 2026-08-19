using System;
using UnityEngine;

public class Seek_Flee : Agent
{
    //Temp
    [SerializeField] private Agent _objetive;

    [SerializeField] private float _maxSpeed = 5;
    [SerializeField] private float _steeringSpeed = 3;
    [SerializeField] private float _maxDistance = 10;
    [SerializeField] private float _minDistance = 0.1f;

    public enum Movement { Flee,Seek,Arrive,Escape,Pursuit,Evade}
    [SerializeField] private Movement _movement;

    void Update()
    {
        MovementTipe();
        transform.position += _actualVelocity * Time.deltaTime;
        transform.forward = _actualVelocity;
    }

    private void MovementTipe()
    {
        switch (_movement) 
        { 
            case Movement.Flee:
                Flee(_objetive.transform.position);
                break;
            case Movement.Seek:
                Seek(_objetive.transform.position);
                break;
            case Movement.Arrive:
                Arrive(_objetive.transform.position);
                break;
            case Movement.Escape:
                Escape(_objetive.transform.position);
                break;
            case Movement.Pursuit:
                Pursuit(_objetive);
                break;
            case Movement.Evade:
                Evade(_objetive);
                break;
        }
                
    }
    //Dado un objetivo, retorna el vector de velocidad deseado que apunta hacia el 
    private Vector3 CalculateDesired(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized;
        return desired * _maxSpeed;
    }

    //Dado un vector de velocidad deseada, retorna un vector de velocidad que tiene en cuenta la capacidad de giro del agente
    private Vector3 CalculateSteering(Vector3 desired)
    {
        desired *= _maxSpeed;
        Vector3 steering = desired - _actualVelocity;
        return Vector3.ClampMagnitude(steering, _steeringSpeed * Time.deltaTime);
    }

    //Modifica la velocidad del agente para que corresponda con seguir al objetivo
    public void Seek(Vector3 target)
    {
        Vector3 desired = CalculateDesired(target);
        _actualVelocity += CalculateSteering(desired);
    }

    public void Flee(Vector3 target)
    {
        Vector3 desired = CalculateDesired(target);
        _actualVelocity -= CalculateSteering(-desired);
    }

    public void Arrive(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        if (distance < _minDistance)
        {
            _actualVelocity = Vector3.zero;
            return;
        }

        float slowSpeed = _maxSpeed * (distance / _maxDistance);

        float desiredSpeed = Mathf.Min(slowSpeed, _maxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed;

        Vector3 Steering = desired - _actualVelocity;

        Steering = Vector3.ClampMagnitude(Steering, _steeringSpeed * Time.deltaTime);

        _actualVelocity += Steering;
    }



    public void Escape(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        if (distance > _maxDistance)
        {
            _actualVelocity = Vector3.zero;
            return;
        }

        float slowSpeed = _maxSpeed * (_minDistance / distance);

        float desiredSpeed = Mathf.Min(slowSpeed, _maxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed * -1;

        Vector3 Steering = desired - _actualVelocity;

        Steering = Vector3.ClampMagnitude(Steering, _steeringSpeed * Time.deltaTime);

        _actualVelocity += Steering;
    }


    private Vector3 CalculateFuture(Agent target)
    {
        Vector3 direction = target.transform.position - transform.position;

        float distance = direction.magnitude;

        float prediction = distance / (_maxSpeed + target._actualVelocity.magnitude);

        return target.transform.position + target._actualVelocity* prediction;

    }
    public void Pursuit(Agent target)
    {
        Vector3 futurePosition = CalculateFuture(target);

        Seek(futurePosition);

    }

    public void Evade(Agent target)
    {
        Vector3 futurePosition = CalculateFuture(target);

        Flee(futurePosition);
    }
}
