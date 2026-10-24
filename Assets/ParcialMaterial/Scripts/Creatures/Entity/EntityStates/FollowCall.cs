using UnityEngine;
using System.Collections.Generic;
public class FollowCall : CreatureState
{
    private Graph _graph = Graph.Instance;

    private MS_Entity _me;
    private StateMachine _machine;
    private FollowCallData _data;
    private PathsManager _pathManager;
    private EntityData _meData;

    private Node _lastKnownPos;

    public FollowCall(EntityData meData,MS_Entity me, FollowCallData data, StateMachine machine, PathsManager pathManager)
    {
        _me = me;
        _machine = machine;
        _data = data;
        _pathManager = pathManager;
        _meData = meData;
    }
    public override void Enter()
    {
        EntityData.HunterIsDead += HunterIsDead;
        _lastKnownPos = Graph.Instance.HunterPos;
        _pathManager.SetStartAndEnd(_me._miNodePos, _lastKnownPos);
        _me._debugger.SetTitle("Someone saw the Hunter", _me.gameObject.name,Color.yellowNice);
    }

    public override void Update()
    {
        if (_lastKnownPos != Graph.Instance.HunterPos && EntityData.EyesOnHunter.Count > 0) 
        {
            _lastKnownPos = Graph.Instance.HunterPos;
            _pathManager.SetStartAndEnd(_me._miNodePos, _lastKnownPos);
        }
        if (_meData.Hunter != null)
        {
            _machine.ChangeState(EntityState.Follow);
            return;
        }
        if (Aproach() || Graph.Instance.HunterPos._type.Equals(NodeType.SafeZone))
        {
            _machine.ChangeState(EntityState.Patrol);
            return;
        }

        _me._debugger.selectedDebug($" Last hunter pos in {Graph.Instance.HunterPos.name}",Color.yellow);
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;

    }

    public override void Exit()
    {
        _pathManager.DeletePath();
    }

    private void HunterIsDead()
    {
        EntityData.HunterIsDead -= HunterIsDead;
        _machine.ChangeState(EntityState.Patrol);
    }

    private bool Aproach()
    {
        float distance = Vector3.Distance(_me.transform.position, _pathManager.Desired());

        if (distance <= _data.MinDistanceToNode)
        {
            
            if (_pathManager.Next())
            {
                return true;
            }
        }
        _me.CustomSpeedSeek(_pathManager.Desired(),_data.FollowSpeed);
        return false;
    }    

}

[System.Serializable]
public class FollowCallData
{
    public float FollowSpeed;
    public float MinDistanceToNode;

}