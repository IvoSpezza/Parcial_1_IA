using System;
using UnityEngine;

public class Boid_AnimationController : MonoBehaviour
{
    private Animator _animator;
    private MS_BoidControlScript _me;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _me = GetComponent<MS_BoidControlScript>();
        _me.OnChangeState += SetAnimation;
    }

    private void  SetAnimation(BoidState state)
    {
        Debug.Log("CAMBIOESTADO");
        _animator.SetBool("IsSwiming", state == BoidState.Flocking);
        _animator.SetBool("IsEvading", state == BoidState.Evading);
        _animator.SetBool("IsAttacking", state == BoidState.Atacking);
        _animator.SetBool("IsDead", state == BoidState.Dead);
    } 
}
