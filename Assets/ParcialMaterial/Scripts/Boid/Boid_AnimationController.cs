using System;
using UnityEngine;

public class Boid_AnimationController : MonoBehaviour
{
    private Animator _animator;
    private StateMachine _animationState;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animationState = GetComponent<MS_BoidControlScript>()._machine;
        _animationState.OnStateChanged += SetAnimation;
    }
       
    private void SetAnimation(Enum state)
    {
        BoidState boidState = (BoidState)state;

        
        _animator.SetBool("IsSwiming", boidState == BoidState.Flocking);
        _animator.SetBool("IsEvading", boidState == BoidState.Evading);
        _animator.SetBool("IsAttacking", boidState == BoidState.Atacking);
        _animator.SetBool("IsDead", boidState == BoidState.Dead);
    } 
}
