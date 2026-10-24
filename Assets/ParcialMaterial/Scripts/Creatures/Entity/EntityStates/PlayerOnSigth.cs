using UnityEngine;

public class PlayerOnSigth : CreatureState
{
    private StateMachine _machine;
    private MS_Entity _me;
    private PlayerOnSigthData _data;
    private EntityData _meData;

    public PlayerOnSigth(EntityData meData,MS_Entity  me, StateMachine machine, PlayerOnSigthData data)
    {
        _me = me;
        _machine = machine;
        _data = data;
        _meData = meData;
    }
    public override void Enter()
    {
        EntityData.EyesOnHunter.Add(_me);
        _me._debugger.SetTitle("Hunting The Hunter", _me.gameObject.name, Color.red);
    }

    public override void Update()
    {
        if (_meData.Hunter == null)
        {
            _machine.ChangeState(EntityState.Patrol);            
            return;
        }
        if (Graph.Instance.HunterPos._type.Equals(NodeType.SafeZone))
        {
            _machine.ChangeState(EntityState.Patrol);
            EntityData.EyesOnHunter.Clear();            
            return;
        }

        Pursuit(_meData.Hunter);
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;

        float distance = Vector3.Distance(_meData.Hunter.transform.position, _me.transform.position);
        _me._debugger.selectedDebug($"Hunter{distance} m. away",Color.darkRed);
        
        if(distance <= _data.MinDistance)
        {            
            _meData.Hunter.Die();
            EntityData.EyesOnHunter.Clear();
            _meData.KillHunter();
            _machine.ChangeState(EntityState.Patrol);
            return;
        }
    }
    

    public override void Exit()
    {
        base.Exit();
    }

    public void Pursuit(MS_Creature target)
    {
        Vector3 futurePosition = _me.CalculateFuture(target);
        Debug.DrawLine(_me.transform.position, futurePosition, Color.white, 0.01f);
        _me.CustomSpeedSeek(futurePosition,_data.ChaseSpeed);

    }
}

[System.Serializable]
public class PlayerOnSigthData
{
    public float ChaseSpeed;
    public float MinDistance;    
}


