using UnityEngine;

public class S_Evading : CreatureState
{
    private MS_BoidControlScript _me;
    private Animator _animation;
    public S_Evading(MS_BoidControlScript  me, float maxSpeed, Animator animation)
    {
        _me = me;        
        _animation = animation;
    }
    public override void Enter()
    {
        _animation.SetBool("IsEvading", true);
    }

    public override void Update()
    {        
        Evade(_me._enemy);
    }
    public override void Exit() 
    {
        _animation.SetBool("IsEvading", false);
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


