using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class S_Atack : CreatureState
{
    private List<MS_BoidControlScript> _boidsInRange;
    private MS_Hunter _me;
    private MS_BoidControlScript _myPrey;
    private AtackDatta _atackData;
    private Action WayOfKilling;
    private float _rangedCharge;

    public event Action<AtackTipe> OnAtackCreature;

    public S_Atack(List<MS_BoidControlScript> boidsInRange, AtackDatta atackData, MS_Hunter me, Animator animator)
    {
        _boidsInRange = boidsInRange;
        _atackData = atackData;
        _me = me;
        _animator = animator;
    }

    public override void Enter()
    {
        _rangedCharge = 0;
        DeterminePrey();        
    }

    private void DeterminePrey()
    {
        if (_boidsInRange.Count == 0)
        {
            OnAtackCreature?.Invoke(AtackTipe.Fail);
            _myPrey = null;
            _animator.SetBool("Aim", false);
            return;
        }

        _myPrey = _boidsInRange[0];
        float minDistance = Vector3.Distance(_me.transform.position, _myPrey.transform.position);
        float checkDistance;
        foreach(MS_BoidControlScript posiblePrey in _boidsInRange)
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
            Debug.DrawLine(_me.transform.position, _myPrey.transform.position, Color.red, 0.01f);
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
        _me.transform.forward = _myPrey.transform.position - _me.transform.position;

        float distanceToPrey = Vector3.Distance(_me.transform.position, _myPrey.transform.position);

        if (distanceToPrey > _atackData._mPr)
        {
            WayOfKilling = KillItRanged;
            return;
        }

        if (distanceToPrey <= _atackData._mad)
        {
            _animator.SetTrigger("Melee");
            Atack(AtackTipe.Succes);
        }
    }

    private void KillItRanged()
    {
        _animator.SetBool("Aim", true);
        _me.AplyVelocity(-_me._velocity);
        _me.transform.forward = _myPrey.transform.position - _me.transform.position;
        _rangedCharge += Time.deltaTime;

        float distanceToPrey = Vector3.Distance(_me.transform.position, _myPrey.transform.position);

        if (distanceToPrey <= _atackData._mPr) WayOfKilling = KillitMelee;        

        if (distanceToPrey > _atackData._rad)
        {
            DeterminePrey();
        }

        if(_rangedCharge >= _atackData._ract)
        {
            _animator.SetTrigger("Shoot");
            _animator.SetBool("Aim", false);
            Atack(AtackTipe.Succes);
        }
    }
    public void Seek(Vector3 target)
    {
        Vector3 desired = _me.CalculateDirection(target);
        _me.AplyVelocity(_me.CalculateSteering(desired));
    }

    private void Atack(AtackTipe atackTipe)
    {
        _boidsInRange.Remove(_myPrey);
        _myPrey.Die();
        OnAtackCreature?.Invoke(atackTipe);
    }
    
    public void Pursuit(MS_Creature target)
    {
        Vector3 futurePosition = _me.CalculateFuture(target);
        Debug.DrawLine(_me.transform.position, futurePosition, Color.white, 0.01f);
        Seek(futurePosition);

    }
    public override void Exit()
    {
        base.Exit();
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

public enum AtackTipe
{
    Succes,
    Fail
}