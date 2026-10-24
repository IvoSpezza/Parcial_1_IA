using System.Collections.Generic;
using UnityEngine;

public class S_GraphHunt : CreatureState
{
    private Graph _graph = Graph.Instance;

    private HuntGraphData _data;
    private MS_Hunter _me;
    private PathsManager _pathManager;

    private int _orientation;
    private int _actualPoint;    

    private float _actualTime;
    private HunterData _hunterData;
    private OP_Pool _trapsPool;
    private float _trapTimer;

    public S_GraphHunt(HuntGraphData data, MS_Hunter me, StateMachine hunterMachine, HunterData hunterData, PathsManager pathsManager, Animator animator)
    {
        _data = data;
        _me = me;
        _stateMachine = hunterMachine;
        _hunterData = hunterData;        
        _pathManager = pathsManager;
        _trapsPool = new OP_Pool(_data._trap, null, _data._maxTraps);
        _animator = animator;

        for (int i = 0; i < _data._maxTraps; i++)
        {
            MS_trapScript trap = _trapsPool.CreateObject().GetComponent<MS_trapScript>();
            trap.SetPool(_trapsPool);
        }
    }

    public override void Enter()
    {
        _animator.SetBool("Patrol", true);
        _pathManager.SetStartAndEnd(_me._actualNode,_graph.GetRamdomNodeInType(NodeType.HuntField));        
    }

 

    public override void Update()
    {        
        Patrol();
        _actualTime += Time.deltaTime;
        _trapTimer += Time.deltaTime;
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;
        if (_hunterData._entitySeen.Count > 0)
        {
            _stateMachine.ChangeState(HunterStates.Flee);
            return;
        }
        TimeTracker();       
        TryAtack();

    }

    public override void Exit()
    {
        _actualTime = 0;
        _pathManager.DeletePath();
    }

    private void TimeTracker()
    {
        if (_hunterData._entitySeen.Count > 0)
        {
            _stateMachine.ChangeState(HunterStates.Flee);
            return;
        }

        if (_trapTimer >= _data._tbt)
        {
            _trapTimer = 0;
            ColocateTrap();
        }

        if (_actualTime >= _data._tba)
        {
            _hunterData._canAtack = true;
            _me.HunterDebDebugger.selectedDebug("COME CLOSE AND I WILL KILL YOU", Color.green);
        }
        else
        {
            _me.HunterDebDebugger.selectedDebug("rechargin atack in: " + (_data._tba - _actualTime) + "\n" +
                                                +_trapsPool._actives + " traps actives. Next in: " + (_data._tbt - _trapTimer), Color.green);
        }
    }
    private void TryAtack()
    {
        if (_hunterData._deadBoids.Count > 0)
        {
            _stateMachine.ChangeState(HunterStates.Recolect);
            return;
        }

        if (_hunterData._canAtack && _hunterData._posiblePreys.Count > 0)
        {
            _stateMachine.ChangeState(HunterStates.Hunt);
        }
    }

    public void Patrol()
    {
        float distance = Vector3.Distance(_me.transform.position, _pathManager.Desired());

        if (distance <= _data._minRangeToChange)
        {
            
            if (_pathManager.Next())
            {                
                _pathManager.SetStartAndEnd(_me._actualNode, _graph.GetRamdomNodeInType(NodeType.HuntField));       
                return;
            }
        }
        _me.CustomSpeedSeek(_pathManager.Desired(), _data._patrolSpeed);
    }
    private void ColocateTrap()
    {
        if (_trapsPool._actives < _data._maxTraps)
        {            
            GameObject trap = _trapsPool.Get();
            trap.transform.position = _me.transform.position;
        }
    }
}

[System.Serializable]
public class HuntGraphData
{    
    public float _minRangeToChange;
    public float _patrolSpeed;
    public float _tba;

    public int _maxTraps;
    public float _tbt;
    public GameObject _trap;
}