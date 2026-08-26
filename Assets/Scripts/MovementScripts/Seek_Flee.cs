using System.Collections.Generic;
using UnityEngine;

public class Seek_Flee : Agent
{
    //Temp
    [SerializeField] private Agent _objetive;


    //Datos para el movimiento
    [SerializeField] private float _maxSpeed = 5;
    [SerializeField] private float _steeringSpeed = 3;
    [SerializeField] private float _maxDistance = 10;
    [SerializeField] private float _minDistance = 0.1f;

    //seleccion de comportamiento
    public enum Movement { Flee,Seek,Arrive,Escape,Pursuit,Evade, Flocking}
    [SerializeField] private Movement _movement;



    //Datos flocking
    private List<Agent> _agents;

    private SphereCollider _sphereCollider;

    [SerializeField, Range(0f, 3f)]private float _cohesionWeigth = 1f;
    [SerializeField, Range(0f, 3f)]private float _aligmentWeigth = 1f;
    [SerializeField, Range(0f, 3f)] private float _separationWeigth = 1f;

    [SerializeField, Range(0f, 3f)] private float _randomFactor = 0f;


    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _agents = new List<Agent>();

        SetStats();
        RandomMovement();
    }

    private void SetStats()
    {
        if (_movement == Movement.Flocking)
        {
            _minDistance = _sphereCollider.radius / 3;
        }
        _maxSpeed += Random.Range(-_randomFactor, _randomFactor);
        _steeringSpeed += Random.Range(-_randomFactor, _randomFactor);
        _minDistance += Random.Range(-_randomFactor, _randomFactor);
    }

    private void RandomMovement()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1, 1), 0f, Random.Range(-1, 1));
        _actualVelocity = randomDirection.normalized * _maxSpeed;
    }

    void Update()
    {
        MovementTipe();
        transform.position += _actualVelocity * Time.deltaTime;
        transform.forward = _actualVelocity;
        transform.position = Bounds.instance.OutOfBounds(transform.position);
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
            case Movement.Flocking:
                Flocking();
                break;
        }
                
    }
    //Dado un objetivo, retorna el vector de velocidad deseado que apunta hacia el 
    private Vector3 CalculateDirection(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        return direction;
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
        Vector3 desired = CalculateDirection(target);
        _actualVelocity += CalculateSteering(desired);
    }

    //Dado un objetivo, escapamos directamente de el
    public void Flee(Vector3 target)
    {
        Vector3 desired = CalculateDirection(target);
        _actualVelocity += CalculateSteering(-desired);
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

        Vector3 Steering = CalculateSteering(desired);

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

        Vector3 Steering = CalculateSteering(desired);
        _actualVelocity += Steering;
    }


    private Vector3 CalculateFuture(Agent target)
    {
        Vector3 direction = target.transform.position - transform.position;

        float distance = direction.magnitude;

        float prediction = distance / (_maxSpeed + target._actualVelocity.magnitude);

        return target.transform.position + target._actualVelocity * prediction;

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

    private Vector3 CalculateSeparation()
    {
        Vector3 dessired = Vector3.zero;

        foreach(Agent agent in _agents)
        {
            if ((agent.transform.position - transform.position).magnitude <= _minDistance)
            {
                dessired += CalculateDirection(agent.transform.position);
            }
        }
        dessired.y = 0;
        return CalculateSteering(-dessired);
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 dessired = Vector3.zero;

        foreach (Agent agent in _agents)
        {
            dessired += agent._actualVelocity.normalized;
        }
        dessired.y = 0;
        return CalculateSteering(dessired.normalized);
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 dessired = Vector3.zero;

        foreach (Agent agent in _agents)
        {
            dessired += CalculateDirection(agent.transform.position);
        }

        dessired.y = 0;
        return CalculateSteering(dessired.normalized);
    }

    private void Flocking()
    {
        _actualVelocity += (CalculateSeparation()* _separationWeigth) + (CalculateAlignment()* _aligmentWeigth) + (CalculateCohesion()* _cohesionWeigth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Agent>(out Agent agenteEnRango))
        {
            _agents.Add(agenteEnRango);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Agent>(out Agent agenteEnRango))
        {
            _agents.Add(agenteEnRango);
            Debug.Log(_agents.Count);
        }
    }

}  
   