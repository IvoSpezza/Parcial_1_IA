using UnityEngine;

public abstract class CreatureState
{
    protected float _maxSpeed;
    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }

}
