using UnityEngine;

public class S_HFlee : CreatureState
{
    private Graph _graph = Graph.Instance;

    private MS_Hunter _me;
    private PathsManager _pathManager;
    private StateMachine _machine;
    private HFleeData _data;
    public S_HFlee(MS_Hunter me, PathsManager pathManager, HFleeData data, StateMachine machine,Animator animation)
    {
        _me = me;
        _pathManager = pathManager;
        _machine = machine;
        _data = data;
        _animator = animation;
    }

    public override void Enter()
    {
        _animator.speed = 6;
        Node closest = _graph.GetClosestTypeNodeZone(_me._actualNode, NodeType.SafeZone);
        Node addDeepnes = _graph.GetDeeperNodeInZone(closest,NodeType.SafeZone, _data.stepsInsideSafe);
        _pathManager.SetStartAndEnd(_me._actualNode, addDeepnes);
        _me.HunterDebDebugger.SetTitle("I HAVE TO RUNNNN FAST FAST FAST","Hunter",Color.rebeccaPurple);
    }
    public override void Update()
    {
        Escape();
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;
        _me.HunterDebDebugger.selectedDebug("Running to safe zone",Color.mediumPurple);
    }

    private void Escape()
    {
        float distance = Vector3.Distance(_me.transform.position, _pathManager.Desired());


        if (distance <= _data.MinDistanceToNode)
        {            
            if (_pathManager.Next())
            {
                _machine.ChangeState(HunterStates.Dead);
                return;
            }
        }
        
        _me.CustomSpeedSeek(_pathManager.Desired(), _data.FleeSpeed);
    }

    public override void Exit()
    {
        _animator.speed = 1;
        _pathManager.DeletePath();
    }
}

[System.Serializable]
public class HFleeData
{
    public float FleeSpeed;
    public float MinDistanceToNode;
    public int stepsInsideSafe;
}
