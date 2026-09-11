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
        
    }
    public override void Update()
    {
        
    }
    public override void Exit()
    {
        base.Exit();
    }
}
