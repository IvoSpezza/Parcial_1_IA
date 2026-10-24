using UnityEngine;

public class S_GoingToHunt : CreatureState
{
    private Graph _graph = Graph.Instance;

    private MS_Hunter _me;
    private PathsManager _pathManager;
    private StateMachine _machine;
    private HunterHuntOtExitData _data;
    private HunterData _entitysData;
    
    public S_GoingToHunt(HunterData entityData,MS_Hunter me, PathsManager pathManager,HunterHuntOtExitData data ,StateMachine machine, Animator animator)
    {
        _me = me;
        _pathManager = pathManager;
        _machine = machine;
        _data = data;
        _animator = animator;
        _entitysData = entityData;
    }

    public override void Enter()
    {
        _animator.SetBool("Patrol", true);
        _pathManager.DeletePath();
        Node closest = _graph.GetClosestTypeNodeZone(_me._actualNode,NodeType.HuntField);
        _pathManager.SetStartAndEnd(_me._actualNode,closest);
        _me.HunterDebDebugger.SetTitle("Going to hunt","Hunter",Color.darkSeaGreen);
        _me.HunterDebDebugger.selectedDebug($"Closest hunt zone: {closest}",Color.dodgerBlue);
    }
    public override void Update()
    {        
        Hunt();
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;



        if (_entitysData._entitySeen.Count > 0)
        {
            _machine.ChangeState(HunterStates.Flee);
            return;
        }

        if (_me._actualNode._type.Equals(NodeType.HuntField))
        {            
            _machine.ChangeState(HunterStates.Hunting);
            return;
        }
    }

    private void Hunt()
    {
        float distance = Vector3.Distance(_me.transform.position, _pathManager.Desired());

        if (distance <= _data.MinDistanceToNode)
        {                            
        
            if (_pathManager.Next())
            {                
                _machine.ChangeState(HunterStates.Hunting);
                return;
            }                     
        }
        _me.CustomSpeedSeek(_pathManager.Desired(), _data.PatrolSpeed);
    }

    public override void Exit()
    {        
        _pathManager.DeletePath();
    }
}

[System.Serializable]
public class HunterHuntOtExitData
{
    public float PatrolSpeed;

    public float MinDistanceToNode;
}