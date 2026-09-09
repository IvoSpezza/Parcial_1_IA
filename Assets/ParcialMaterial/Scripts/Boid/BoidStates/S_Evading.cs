using UnityEngine;

public class S_Evading : CreatureState
{
    private MS_BoidControlScript _me;
    private MS_Hunter _enemy;
    public S_Evading(MS_BoidControlScript  me, MS_Hunter enemy)
    {
        _me = me;
        _enemy = enemy;
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        Evade(_enemy);
        _me.transform.position += _velocity;
        _me.SetVelocity(_velocity);
    }
    public override void Exit() 
    {
        base.Exit(); 
    }

    public void Flee(Vector3 target)
    {
        Vector3 desired = (target - _me.transform.position).normalized;
        _velocity += _me.CalculateSteering(-desired);
    }
    private Vector3 CalculateFuture(MS_Creature target)
    {
        Vector3 direction = target.transform.position - _me.transform.position;

        float distance = direction.magnitude;

        float prediction = distance / (_maxSpeed + target._velocity.magnitude);

        return target.transform.position + target._velocity * prediction;

    }
    public void Evade(MS_Creature target)
    {
        Vector3 futurePosition = CalculateFuture(target);

        Flee(futurePosition);
    }
}


