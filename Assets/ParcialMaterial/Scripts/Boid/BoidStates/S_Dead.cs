using UnityEngine;

public class S_Dead : CreatureState
{

    private MS_BoidControlScript _me;

    public S_Dead(MS_BoidControlScript me)
    {
        _me = me;
    }
    public override void Enter()
    {
        _me.AplyVelocity(-_me._velocity);
    }
    public override void Update()
    {
        _me.AplyVelocity(-_me._velocity);
    }
    public override void Exit()
    {
        base.Exit();
    }
}
