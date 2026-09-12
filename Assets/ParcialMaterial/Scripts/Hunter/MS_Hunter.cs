using System.Collections.Generic;
using UnityEngine;

public class MS_Hunter : MS_Creature
{
    [SerializeField] private float _tba;
    [SerializeField] private float _rangeOfDetection;

    [Header("States Data")]
    [SerializeField] private PatrolData _patrol;
    [SerializeField] private AtackDatta _atack;
    [SerializeField] private RecolectData _recolectData;

    private List<MS_BoidControlScript> _posiblePreys;
    private Stack<MS_BoidControlScript> _deadBoids;
    private StateMachine _hunterMachine;

    private bool _timeToAtack;
    private bool _preysToAtack;
    
    private SphereCollider _viewDistance;
    
    private void Awake()
    {
        Animator animation = GetComponent<Animator>();

        _timeToAtack = false;

        _viewDistance = GetComponent<SphereCollider>();
        _viewDistance.radius = _rangeOfDetection;

        _posiblePreys = new List<MS_BoidControlScript>();
        _deadBoids = new Stack<MS_BoidControlScript>();

        _hunterMachine = new StateMachine();
        S_Patrol patrolState = new S_Patrol(_patrol,this,_tba);
        patrolState.CanAtack += CanAtack;
        _hunterMachine.AddState(patrolState, HunterStates.Patrol);

        S_Atack atacState = new S_Atack(_posiblePreys, _atack,this,animation);
        atacState.OnAtackCreature += ChangeToPatrol;
        _hunterMachine.AddState(atacState, HunterStates.Hunt);

        S_Recolect recolect = new S_Recolect(this, _recolectData, _deadBoids,animation);
        recolect.OnRecolect += ChangeToPatrol;
        _hunterMachine.AddState(recolect, HunterStates.Recolect);

        _hunterMachine.ChangeState(HunterStates.Patrol);
    }

    

    private void Update()
    {
        _hunterMachine.MachineUpdate();
        transform.position = Bounds.instance.OutOfBounds(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject boid = other.gameObject.transform.parent.gameObject;

        if(boid.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript  prey))
        {
            if (!prey._isAlive)
            {
                _deadBoids.Push(prey);
                _velocity = Vector3.zero;
                _hunterMachine.ChangeState(HunterStates.Recolect);
            }

            _posiblePreys.Add(prey);
            if(_posiblePreys.Count >= 1)
            {
                _preysToAtack = true;
                TryAtack();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject boid = other.gameObject.transform.parent.gameObject;

        if (boid.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
            if (!prey._isAlive) return;
            _posiblePreys.Remove(prey);
            if (_posiblePreys.Count == 0)
            {
                _preysToAtack = false;
            }
        }
    }

    //Time to atack charged
    private void CanAtack()
    {
        _timeToAtack = true;
        if (_preysToAtack)
        {
            TryAtack();
        }
    }

    //With someone on range tries to atack if atack coldown has pased
    private void TryAtack()
    {
        if(_timeToAtack && _preysToAtack)
        { 
            _hunterMachine.ChangeState(HunterStates.Hunt);
        }
    }

    private void ChangeToPatrol(bool resetTTA)
    {
        if(!resetTTA)
        {           
            _hunterMachine.ChangeState(HunterStates.Patrol);
        }
        else
        {
            _timeToAtack = false;
            _hunterMachine.ChangeState(HunterStates.Patrol);
        }
    }

}

public enum HunterStates
{
    Patrol,
    Recolect,
    Hunt,
    Atack
}
