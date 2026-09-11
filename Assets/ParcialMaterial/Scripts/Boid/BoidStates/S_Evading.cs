using UnityEngine;

public class S_Evading : CreatureState
{
    private MS_BoidControlScript _me;
    public S_Evading(MS_BoidControlScript  me, float maxSpeed)
    {
        _me = me;
        _maxSpeed = maxSpeed;
    }
    public override void Enter()
    {
      
    }

    public override void Update()
    {        
        Evade(_me._enemy);
    }
    public override void Exit() 
    {
        base.Exit(); 
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


