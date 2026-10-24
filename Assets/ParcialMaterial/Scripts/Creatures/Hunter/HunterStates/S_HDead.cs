using UnityEngine;

public class S_HDead : CreatureState
{

    private MS_Hunter _me;
    private DeadData _deadData;
    private HunterData _objetiveData;
    private StateMachine _machine;

    private float _localTime;
    private float _waitTime;
    public S_HDead(HunterData meData,MS_Hunter me,StateMachine machine, DeadData deadData,Animator animation)
    {
        _me = me;        
        _animator = animation;
        _deadData = deadData;
        _machine = machine;
        _objetiveData = meData;
    }
    public override void Enter()
    {
        _animator.SetBool("Patrol", false);

        _waitTime = _me._alive ? _deadData.SafeZoneTime : _deadData.DeadTime;
        _me.HunterDebDebugger.SetTitle($"I am {(_me._alive ? "waiting" : "dead")}", "Hunter", Color.darkRed);
    }
    public override void Update()
    {
        _me.HunterDebDebugger.selectedDebug($"I will keep goin in {(_me._alive ? _deadData.SafeZoneTime : _deadData.DeadTime) - _localTime} seconds",Color.darkSalmon);
        _localTime += Time.deltaTime;
        if (_localTime > _waitTime) 
        {
            _localTime = 0;
            if (_me._alive && _objetiveData._maxPointsReached)
            {                
                _machine.ChangeState(HunterStates.Exit);               
            }
            else
            {
                _machine.ChangeState(HunterStates.HuntFieldSeek);
            }
        }
    }
    public override void Exit()
    {
        _animator.SetBool("Patrol", true);
        _me._alive = true;
    }

}

[System.Serializable]
public class DeadData
{
    public float DeadTime;
    public float SafeZoneTime;
}