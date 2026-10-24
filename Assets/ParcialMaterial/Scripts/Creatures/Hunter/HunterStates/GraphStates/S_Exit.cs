using UnityEngine;

public class S_Exit : CreatureState
{
    private Graph _graph = Graph.Instance;

    private MS_Hunter _me;
    private PathsManager _pathManager;
    private StateMachine _machine;
    private HunterHuntOtExitData _data;
    private HunterData _entityData;
    public S_Exit(HunterData entityData,MS_Hunter me, PathsManager pathManager,HunterHuntOtExitData data ,StateMachine machine,Animator animator)
    {
        _me = me;
        _pathManager = pathManager;
        _machine = machine;
        _data = data;
        _animator = animator;
        _entityData = entityData;
    }

    public override void Enter()
    {       
        _animator.SetBool("Patrol", true);
        Node closest = _graph.GetClosestTypeNodeZone(_me._actualNode,NodeType.Finish);
        _pathManager.SetStartAndEnd(_me._actualNode, closest);        
    }
    public override void Update()
    {
        if (_entityData._entitySeen.Count > 0)
        {
            _machine.ChangeState(HunterStates.Flee);
            return;
        }
        Escape();
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;

    }

    private void Escape()
    {
        float distance = Vector3.Distance(_me.transform.position, _pathManager.Desired());

        if (distance <= _data.MinDistanceToNode)
        {
            if (_pathManager.Next())
            {                                
                _me.Die(true);
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

