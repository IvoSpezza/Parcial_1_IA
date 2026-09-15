using UnityEngine;

public abstract class CreatureState
{
    protected Animator _animator;
    protected StateMachine _stateMachine;
    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }

}
