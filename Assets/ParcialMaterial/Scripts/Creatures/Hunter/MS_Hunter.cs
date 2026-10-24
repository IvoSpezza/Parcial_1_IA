using System;
using System.Collections.Generic;
using UnityEngine;

public class MS_Hunter : MS_Creature
{    
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private int _bulletsAmount;
    [SerializeField] private GameObject _bulletPrefab;
    private OP_Pool _bulletsPool;

    [Header("States Data")]
    //[SerializeField] private PatrolData _patrol;
    [SerializeField] private HunterHuntOtExitData _patrolData;
    [SerializeField] private HuntGraphData _huntGraphData;
    [SerializeField] private AtackDatta _atack;
    [SerializeField] private RecolectData _recolectData;
    [SerializeField] private HFleeData _hleeData;
    [SerializeField] private DeadData _deadData;

    [SerializeField] private Color _pathColor;
    private PathsManager _pathsManager;

    [SerializeField] private NodeTracker _nodeTracker;
    public Node _actualNode { get; protected set; }
    private void SetNode(Node node) =>_actualNode = node;            

    [SerializeField] public MS_CreatureDebugger HunterDebDebugger;

    private FieldOfView _fieldOfView;

    private StateMachine _hunterMachine;
    public HunterData _hunterData { get; private set; }

    public event Action OnDie;
    public bool _alive  = true;

    private void Awake()
    {
        _nodeTracker.OnAnotherNode += SetNode;
        _fieldOfView = GetComponent<FieldOfView>();

        _hunterMachine = new StateMachine();     
               

        _bulletsPool = new OP_Pool(_bulletPrefab, transform.parent, _bulletsAmount);        
    }

    private void OnDestroy()
    {
        _nodeTracker.OnAnotherNode -= SetNode;
    }

    private void SetupBasicStates()
    {
        Animator animation = GetComponent<Animator>();

        /*
        S_Patrol patrolState = new S_Patrol(_patrol, this, _hunterMachine, _hunterData, animation);
        _hunterMachine.AddState(patrolState, HunterStates.Patrol);
        */

        S_GoingToHunt searHuntPlace = new S_GoingToHunt(_hunterData,this,_pathsManager,_patrolData,_hunterMachine,animation);
        _hunterMachine.AddState(searHuntPlace, HunterStates.HuntFieldSeek);

        S_GraphHunt graphHunt = new S_GraphHunt(_huntGraphData, this, _hunterMachine, _hunterData,_pathsManager,animation);
        _hunterMachine.AddState(graphHunt, HunterStates.Hunting);

        S_Atack atacState = new S_Atack(_atack, this, _hunterMachine, _hunterData, animation);
        _hunterMachine.AddState(atacState, HunterStates.Hunt);

        S_Recolect recolect = new S_Recolect(_recolectData,this,_hunterMachine,_hunterData, animation);        
        _hunterMachine.AddState(recolect, HunterStates.Recolect);

        S_Exit exit = new S_Exit(_hunterData, this,_pathsManager, _patrolData,_hunterMachine,animation);
        _hunterMachine.AddState(exit, HunterStates.Exit);

        S_HFlee flee = new(this,_pathsManager,_hleeData,_hunterMachine,animation);
        _hunterMachine.AddState(flee, HunterStates.Flee);

        S_HDead deadOrWin = new S_HDead(_hunterData,this,_hunterMachine,_deadData,animation);
        _hunterMachine.AddState(deadOrWin, HunterStates.Dead);
    }

    private void SetupHunterData()
    {
        _hunterData = new HunterData();
        _hunterData._posiblePreys = new List<MS_BoidControlScript>();
        _hunterData._deadBoids = new List<MS_BoidControlScript>();
        _hunterData._canAtack = false;
    }
    private void Start()
    {
        MS_Bullet bullet;
        for(int i = 0;i<_bulletsAmount ; i++)
        {
            bullet = _bulletsPool.CreateObject().GetComponent<MS_Bullet>();
            bullet.SetPool(_bulletsPool);
        }

        _actualNode = Graph.Instance.GetNearestNode(transform.position);

        _pathsManager = new(new(_pathColor),_actualNode);

        SetupHunterData();
        SetupBasicStates();       

        _hunterMachine.ChangeState(HunterStates.HuntFieldSeek);
    }

    private void Update()
    {
        AvoidObstacles(_fieldOfView.EyeHeight, _fieldOfView.ObstacleMask);
        _hunterMachine.MachineUpdate();        
    }      

    public void Shoot(Vector3 objetibe)
    {        
        GameObject bullet = _bulletsPool.Get();
        bullet.transform.position = _shootingPoint.position;
        bullet.transform.forward = (objetibe - bullet.transform.position).normalized;
    }

    public void Die(bool reset = false)
    {
        OnDie?.Invoke();
        _alive = false;        
        _hunterMachine.ChangeState(HunterStates.Dead);
        if (reset) _hunterData._actualPoints = 0;
        _actualNode = Graph.Instance.GetNearestNode(transform.position);
    }    
}
public class HunterData
{
    public List<MS_BoidControlScript> _posiblePreys;
    public List<MS_BoidControlScript> _deadBoids;
    public HashSet<MS_Entity> _entitySeen;
    public bool _canAtack;
    public int _actualPoints = 0;
    public bool _maxPointsReached;
}

public enum HunterStates
{
    Patrol,
    Recolect,
    Hunt,
    Atack,

    HuntFieldSeek,
    Hunting,
    Flee,
    Exit,

    Dead
}
