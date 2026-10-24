using System;
using System.Collections.Generic;
using UnityEngine;

public class MS_Entity : MS_Creature
{
    [SerializeField] private NodeTracker _myNodeTracker;   

    [SerializeField] private EntityPatrolWayPoints _patrolData;
    [SerializeField] private FollowCallData _followData;
    [SerializeField] private PlayerOnSigthData _playerOnSigthData;
    [SerializeField] public MS_CreatureDebugger _debugger;

    [SerializeField] private Color _pathColor;
    private PathsManager _pathManager;

    private EntityView _entityView;
    private StateMachine _machine;
    public EntityData _entityData { get; private set; }
    public Node _miNodePos {  get; private set; }   

    private void Awake()
    {
        _entityView = GetComponent<EntityView>();
        _myNodeTracker.OnAnotherNode += SetMyNode;              
               
    }

    private void Start()
    {
        _pathManager = new(new(_pathColor), _miNodePos, NodeType.SafeZone);
        _machine = new StateMachine();
        _entityData = new EntityData();

        Patrool patrolState = new(_entityData,this, _patrolData, _machine, _pathManager);
        _machine.AddState(patrolState,EntityState.Patrol);

        FollowCall followState = new(_entityData, this, _followData, _machine, _pathManager);
        _machine.AddState(followState,EntityState.Hunt);

        PlayerOnSigth seePlayerData = new(_entityData, this, _machine,_playerOnSigthData);
        _machine.AddState(seePlayerData, EntityState.Follow);                                            
               
        _machine.ChangeState(EntityState.Patrol);        
    }  

    private void OnDisable()
    {
        _myNodeTracker.OnAnotherNode -= SetMyNode;
    }
    public void SetMyNode(Node node)
    {
        _miNodePos = node;
    }
    private void Update()
    {
        AvoidObstacles(_entityView.EyeHeight, _entityView.ObstacleMask);
        _machine.MachineUpdate();
    }       
}

public class EntityData
{
    public static event Action HunterIsDead;
    public static HashSet<MS_Entity> EyesOnHunter = new();
    public MS_Hunter Hunter = null;

    public void KillHunter()
    {
        HunterIsDead?.Invoke();
    }
}


public enum EntityState
{
    Patrol,
    Hunt,
    Follow
}



