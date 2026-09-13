using System;
using System.Collections.Generic;
using UnityEngine;

public class S_Atack : CreatureState
{
    private HunterData _hunterData;
    private StateMachine _hunterMachine;
    private MS_Hunter _me;
    private MS_BoidControlScript _myPrey;
    private AtackDatta _atackData;

    private Action WayOfKilling;
    private float _rangedCharge;
    private bool _canShoot;

    public S_Atack(AtackDatta atackData, MS_Hunter me,StateMachine hunterMachine, HunterData hunterData, Animator animator)
    {
        _hunterData = hunterData;
        _hunterMachine = hunterMachine;
        _atackData = atackData;
        _me = me;
        _animator = animator;
    }

    public override void Enter()
    {        
        _canShoot = true;
        _rangedCharge = 0;
        DeterminePrey();        
    }

    private void DeterminePrey()
    {
        if (_hunterData._posiblePreys.Count == 0)
        {            
            _myPrey = null;
            _animator.SetBool("Aim", false);
            _hunterMachine.ChangeState(HunterStates.Patrol);
            return;
        }

        _myPrey = _hunterData._posiblePreys[0];
        float minDistance = Vector3.Distance(_me.transform.position, _myPrey.transform.position);
        float checkDistance;
        foreach(MS_BoidControlScript posiblePrey in _hunterData._posiblePreys)
        {
            checkDistance = Vector3.Distance(_me.transform.position, posiblePrey.transform.position);
            if (checkDistance < minDistance)
            {
                _myPrey = posiblePrey;
                minDistance = checkDistance;
            }
        }
        DetermineWayOfKilling();        
    }

    public override void Update()
    {
        if (_myPrey != null)
        {           
            WayOfKilling();
        }
    }

    private void DetermineWayOfKilling()
    {
        float distanceToPrey = Vector3.Distance(_me.transform.position, _myPrey.transform.position);
        if (distanceToPrey <= _atackData._mPr)
        {
            WayOfKilling = KillitMelee;
        }
        else if (distanceToPrey > _atackData._mPr && distanceToPrey <= _atackData._rad)
        {
            WayOfKilling = KillItRanged;
        }
    }
    private void KillitMelee() 
    {
        _animator.SetBool("Aim", false);
        Pursuit(_myPrey);        
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me.CalculateSteering(_myPrey.transform.position - _me.transform.position);

        float distanceToPrey = Vector3.Distance(_me.transform.position, _myPrey.transform.position);

        if (distanceToPrey > _atackData._mPr)
        {
            WayOfKilling = KillItRanged;
            return;
        }

        if (distanceToPrey <= _atackData._mad)
        {
            _animator.SetTrigger("Melee");
            MeleAtack();
        }
    }

    private void KillItRanged()
    {
        _animator.SetBool("Aim", true);
        _me.AplyVelocity(-_me._velocity);
        _me.transform.forward = _me.CalculateSteering(_myPrey.transform.position - _me.transform.position);
        _rangedCharge += Time.deltaTime;

        float distanceToPrey = Vector3.Distance(_me.transform.position, _myPrey.transform.position);
        
        if (distanceToPrey <= _atackData._mPr) WayOfKilling = KillitMelee;        

        if (distanceToPrey > _atackData._rad)
        {
            DeterminePrey();
        }

        if(_rangedCharge >= _atackData._ract && _canShoot)
        {
            _canShoot = false;
            _animator.SetTrigger("Shoot");
            _animator.SetBool("Aim", false);
            _me.Shoot(_me.CalculateFuture(_myPrey));
            _hunterData._canAtack = false;
            _hunterMachine.ChangeState(HunterStates.Patrol);
        }
    }
    public void Seek(Vector3 target)
    {
        Vector3 desired = _me.CalculateDirection(target);
        _me.AplyVelocity(_me.CalculateSteering(desired));
    }

    private void MeleAtack()
    {
        _myPrey.Die();
        _hunterData._deadBoids.Add(_myPrey);
        _hunterData._posiblePreys.Remove(_myPrey);
        _hunterData._canAtack = false;
        Debug.Log(_hunterData._canAtack);
        _hunterMachine.ChangeState(HunterStates.Recolect);
    }
    
    public void Pursuit(MS_Creature target)
    {
        Vector3 futurePosition = _me.CalculateFuture(target);
        Debug.DrawLine(_me.transform.position, futurePosition, Color.white, 0.01f);
        Seek(futurePosition);

    }
    public override void Exit()
    {
        _myPrey = null;
        
    }

}

[System.Serializable]
//_mPr = melee pursuit range
//_ract = ranged atack carge time
public class AtackDatta
{
    public float _mPr;
    public float _mad;

    public float _ract;
    public float _rad;
}