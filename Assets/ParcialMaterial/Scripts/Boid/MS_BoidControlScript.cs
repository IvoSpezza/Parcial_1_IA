using System;
using System.Collections.Generic;
using UnityEngine;

//Mono Behabiur para controloar a los boids
public class MS_BoidControlScript : MS_Creature
{
    [SerializeField] public MS_CreatureDebugger CreatureDebugger;
    [SerializeField] private float _maxSeeDistance;

    [Header("States Data")]
    [SerializeField] private FlockingData _dataForFlocking;
    [SerializeField] private EvadeData _evadeData;
    [SerializeField] private AtackTrapData _boidTrapData;
     
    public StateMachine _machine { get; private set; }

    private BoidData _dataForBoid;
    public bool _isAlive { get; private set; }

    private SphereCollider _collision;

    private OP_Pool _dadPool;
    public event Action OnBodyRecolected;

    private void Awake()
    {
        _isAlive = true;
        _collision = GetComponent<SphereCollider>();

        _collision.radius = _maxSeeDistance;

        _machine = new StateMachine();

        _dataForBoid = new BoidData();
        _dataForBoid._nearAgents = new List<MS_BoidControlScript>();
        _dataForBoid._hunter = new Stack<MS_Hunter>();
        _dataForBoid._maxSpeed = _maxSpeed;
        _dataForBoid._steering = _steering;
        _dataForBoid._nearbyTraps = new List<MS_trapScript>();

        Animator animation = GetComponent<Animator>();

        S_Flocking flocking = new S_Flocking(this,_dataForBoid, _dataForFlocking,_machine, animation);
        S_Evading evade = new S_Evading(this, _evadeData, _dataForBoid,_machine, animation);
        S_Dead dead = new S_Dead(this,animation);
        S_AtackTrap trapState = new S_AtackTrap(_boidTrapData,this,_dataForBoid,_machine,animation);

        _machine.AddState(flocking, BoidState.Flocking);
        _machine.AddState(evade, BoidState.Evading); 
        _machine.AddState(dead, BoidState.Dead);
        _machine.AddState(trapState, BoidState.Atacking);

        _machine.ChangeState(BoidState.Flocking);
    }

    private void Start()
    {
        RandomMovement();
    }

    public void SetPool(OP_Pool myPool)
    {
        _dadPool = myPool;
    }

    private void Update()
    {
        _machine.MachineUpdate();
        if (_isAlive)
        {
            AplyVelocity(CalculateSeparation() * _dataForFlocking._separationWeight);
            transform.position = Bounds.instance.OutOfBounds(transform.position);
        }        
    }


    private void OnTriggerEnter(Collider other)
    {        
        if (!_isAlive) return;

        if (other.gameObject.TryGetComponent<MS_Hunter>(out MS_Hunter enemy))
        {
            _dataForBoid._hunter.Push(enemy);    
            return;
        }
            
        if(other.gameObject.TryGetComponent<MS_trapScript>(out MS_trapScript trap))
        {
            
            _dataForBoid._nearbyTraps.Add(trap);
            return;
        }

        if (other.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript anotherAgent))
        {                     
            if (!anotherAgent._isAlive) return;            
            _dataForBoid._nearAgents.Add(anotherAgent);

        }               
    }
   
    private void OnTriggerExit(Collider other)
    {
        if (!_isAlive) return;

        // The hunter is poped by the evade state.

        if (other.TryGetComponent<MS_trapScript>(out MS_trapScript trap))
        {
            _dataForBoid._nearbyTraps.Remove(trap);
        }

        if (other.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript anotherAgent))
        {
            if (!anotherAgent._isAlive) return;
            _dataForBoid._nearAgents.Remove(anotherAgent);
        }
    }

    public void RandomMovement()
    {
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(0, 2) * 2 - 1, 0f, UnityEngine.Random.Range(0, 2) * 2 - 1);
        AplyVelocity(randomDirection.normalized * _maxSpeed);
    }

    //Extraje la separacion para que siempre evite colisionar, sin importar el estado
    private Vector3 CalculateSeparation()
    {
        Vector3 dessired = default;
        Vector3 ponderatedSep;
        if (_dataForBoid._nearAgents.Count == 0)
        {
            return dessired;
        }

        foreach (MS_BoidControlScript agent in _dataForBoid._nearAgents)
        {
            if (Vector3.Distance(agent.transform.position, transform.position) <= _dataForFlocking._minEvadeDistance)
            {
                ponderatedSep = transform.position - agent.transform.position;
                ponderatedSep /= Vector3.Distance(transform.position, agent.transform.position);
                dessired += ponderatedSep;
            }
        }
        dessired.y = 0;
        dessired /= _dataForBoid._nearAgents.Count;

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

    public void Recolect()
    {
        OnBodyRecolected?.Invoke();
        _dadPool.Disable(this.gameObject);
        CreatureDebugger.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        _machine.ChangeState(BoidState.Flocking);
        RandomMovement();
        _isAlive = true;
        CreatureDebugger.gameObject.SetActive(true);

    }

}

public class BoidData
{
    public List<MS_BoidControlScript> _nearAgents;
    public List<MS_trapScript> _nearbyTraps;
    public Stack<MS_Hunter> _hunter;
    public float _maxSpeed;
    public float _steering;
}

public enum BoidState
{
    Flocking,
    Evading,
    Atacking,
    Dead
}
