using System.Collections.Generic;
using UnityEngine;

public class MS_BoidControlScript : MS_Creature
{
    [SerializeField] private float _minSpeed;

    [SerializeField] private float _maxSeeDistance;    
    [SerializeField] private FlockingData _dataForFlocking;

    public StateMachine _machine {  get; private set; }

    private List<MS_BoidControlScript> _agents;
    public bool _isAlive { get; private set; }

    public MS_Hunter _enemy {  get; private set; }
    private SphereCollider _collision;

    private void Awake()
    {
        _isAlive = true;
        _collision = GetComponent<SphereCollider>();

        _collision.radius = _maxSeeDistance;

        _machine = new StateMachine();

        _agents = new List<MS_BoidControlScript>();

        S_Flocking flocking = new S_Flocking(this, _agents, _dataForFlocking, _maxSpeed, _steering);

        S_Evading evade = new S_Evading(this,_maxSpeed);

        _machine.AddState(flocking, BoidState.Flocking);
        _machine.AddState(evade, BoidState.Evading);

        S_Dead dead = new S_Dead(this);
        _machine.AddState(dead, BoidState.Dead);
        _machine.ChangeState(BoidState.Flocking);
    }

    private void Start()
    {
        RandomMovement();
    }

    private void Update()
    {
        _machine.MachineUpdate();
        if (_isAlive)
        {
            _velocity += CalculateSeparation() * _dataForFlocking._separationWeight;

            SetMinSpeed();

            transform.position += _velocity * Time.deltaTime;
            transform.forward = _velocity;
            transform.position = Bounds.instance.OutOfBounds(transform.position);                  
        }                      
        
    }

    private void SetMinSpeed()
    {
        if (_velocity.sqrMagnitude <= (_minSpeed * _minSpeed))
        {
            Vector3 fallbackDirection = transform.forward;
            _velocity = fallbackDirection * _maxSpeed;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isAlive) return;
        if (other.gameObject.TryGetComponent<MS_Hunter>(out MS_Hunter enemy))
        {
            _enemy = enemy;
            _machine.ChangeState(BoidState.Evading);
        }

        if (other.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript anotherAgent))
        {

            if (!anotherAgent._isAlive) return;
            _agents.Add(anotherAgent);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isAlive) return;
        if (other.gameObject.TryGetComponent<MS_Hunter>(out MS_Hunter enemy) )
        {
            _enemy = null;
            _machine.ChangeState(BoidState.Flocking);
        }

        if (other.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript anotherAgent) )
        {
            if (!anotherAgent._isAlive) return;
            _agents.Remove(anotherAgent);            
        }
    }

    private void RandomMovement()
    {
        Vector3 randomDirection = new Vector3(Random.Range(0, 2) * 2 -1, 0f, Random.Range(0, 2) * 2 - 1);
        AplyVelocity(randomDirection.normalized * _maxSpeed);
    }

    //Extraje la separacion para que siempre evite colisionar, sin importar el estado
    private Vector3 CalculateSeparation()
    {
        Vector3 dessired = default;
        if (_agents.Count == 0)
        {
            return dessired;
        }

        foreach (MS_BoidControlScript agent in _agents)
        {
            if (Vector3.Distance(agent.transform.position, transform.position) <= _dataForFlocking._minEvadeDistance)
            {
                dessired += transform.position - agent.transform.position;
            }
        }
        dessired.y = 0;
        
        if (dessired.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }
        
        return CalculateSteering(dessired.normalized);
    }

    public void Die()
    {
        _velocity = Vector3.zero;
        _machine.ChangeState(BoidState.Dead);
        _isAlive = false;
    }

}

public enum BoidState
{
    Flocking,
    Evading,
    Atacking,
    Dead
}
