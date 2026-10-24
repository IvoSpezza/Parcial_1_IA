using UnityEngine;

public class S_Evading : CreatureState
{
    private MS_BoidControlScript _me;
    private BoidData _boidData;
    private Animator _animation;
    private MS_Hunter _hunter;
    private EvadeData _evadeData;

    public S_Evading(MS_BoidControlScript  me,EvadeData evadeData,BoidData boidData, StateMachine stateMachine, Animator animation)
    {
        _me = me;        
        _boidData = boidData;
        _stateMachine = stateMachine;
        _animation = animation;
        _evadeData = evadeData;
    }
    public override void Enter()
    {
        _me.CreatureDebugger.SetTitle("Evading", _me.gameObject.name, Color.yellow);
        _hunter = _boidData._hunter.Pop();
        _animation.SetBool("IsEvading", true);
    }

    public override void Update()
    {        
        Evade(_hunter);
        _me.CreatureDebugger.selectedDebug("Evading the hunter", Color.yellow);
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;
        CheckEvade();
    }
    public override void Exit() 
    {
        _hunter = null;
        _animation.SetBool("IsEvading", false);
    }

    private void CheckEvade()
    {
        float distance = (_hunter.transform.position - _me.transform.position).sqrMagnitude;

        if (distance > _evadeData._distanceToEvade * _evadeData._distanceToEvade)
        {
            _stateMachine.ChangeState(BoidState.Flocking);
        }
    }

    
    public void Flee(Vector3 target)
    {
        Vector3 desired = (target - _me.transform.position).normalized;

        _me.AplyVelocity(_me.CalculateSteering(-desired));
    }
  
    public void Evade(MS_Creature target)
    {
        Vector3 futurePosition = _me.CalculateFuture(target);

        Flee(futurePosition);
    }
}

[System.Serializable]
public class EvadeData
{
    public float _distanceToEvade;
}


