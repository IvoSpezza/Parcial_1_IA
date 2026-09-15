using UnityEngine;

public class S_Dead : CreatureState
{

    private MS_BoidControlScript _me;
    private Animator _animation;

    public S_Dead(MS_BoidControlScript me, Animator animation)
    {
        _me = me;
        _animation = animation;
    }
    public override void Enter()
    {
        _me.CreatureDebugger.SetTitle("DEAD", "SAD", Color.red);
        _me.CreatureDebugger.selectedDebug("I'm so sad, my wife will miss me", Color.red);
        _animation.SetBool("IsDead", true);
    }
    public override void Update()
    {
        
    }
    public override void Exit()
    {       
        _animation.SetBool("IsDead", false);
    }

    
}
