using System.Collections.Generic;
using UnityEngine;

public class S_AtackTrap : CreatureState
{

    private MS_BoidControlScript _me;
    private AtackTrapData _data;
    private MS_trapScript _actualTrap;
    private BoidData _boidData;

    private float _atackTimer;
    private int _trapsDestroyed;
   

    public S_AtackTrap(AtackTrapData boidAtackData,MS_BoidControlScript me,BoidData boidData,StateMachine stateMachine,Animator animation)
    {
        _me = me;
        _data = boidAtackData;
        _stateMachine = stateMachine;
        _boidData = boidData;
        _animator = animation;
        _trapsDestroyed = 0;
    }

    public override void Enter()
    {
        _me.CreatureDebugger.SetTitle("I SEE TRAP","Points: "+ _trapsDestroyed,Color.darkOrange);
        _atackTimer = 0;
        if (DetermineTrap()) return;        
        _me.CreatureDebugger.selectedDebug("Getting closer", Color.darkOrange);
        _animator.SetBool("IsSwiming", true);
    }

    private bool DetermineTrap()
    {
        if (_boidData._nearbyTraps.Count == 0)
        {
            _actualTrap = null;
            _stateMachine.ChangeState(BoidState.Flocking);
            return true;
        }

        _actualTrap = _boidData._nearbyTraps[0];
        float minDistance = (_actualTrap.transform.position -_me.transform.position).sqrMagnitude;
        float checkDistance;
        foreach (MS_trapScript closestTrap in _boidData._nearbyTraps)
        {
            checkDistance = (closestTrap.transform.position - _me.transform.position).sqrMagnitude;
            if (checkDistance < minDistance)
            {
                _actualTrap = closestTrap;
                minDistance = checkDistance;
            }
        }
        return false;
    }

    public override void Update()
    {
        _me.Arrive(_actualTrap.transform.position ,_data._minDistance,_data._distanceToSlow);
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;

        if(CheckSurrondings()) return;

        if (_actualTrap == null)
        {
            _stateMachine.ChangeState(BoidState.Flocking);            
            return;
        } 
        else if(_actualTrap.Life <= 0)
        {
            _boidData._nearbyTraps.Remove(_actualTrap);
            _trapsDestroyed++;
            _stateMachine.ChangeState(BoidState.Flocking);            
            return;
        }
                
        if (_me._velocity == Vector3.zero)
        {            
            Vector3 fowar = _actualTrap.transform.position - _me.transform.position;
            fowar.y = 0;
            _me.transform.forward = fowar;
            _animator.SetBool("IsSwiming", false);
            _animator.SetBool("IsAtacking", true);
            _atackTimer += Time.deltaTime;

            if (_atackTimer >= _data._tba && _actualTrap.Life > 0)
            {
                _atackTimer = 0;
                _actualTrap.Hit(_data._damage);
                _me.CreatureDebugger.selectedDebug("Trap life: " + _actualTrap.Life, Color.darkOrange);
            }                                       
        }
    }

    private bool CheckSurrondings()
    {
        if (_boidData._hunter.TryPeek(out MS_Hunter hunter))
        {
            _stateMachine.ChangeState(BoidState.Evading);
            return true;
        }
        return false;
    }

    public override void Exit()
    {
        _animator.SetBool("IsAtacking", false);                
        _actualTrap = null;
    }
        
}

[System.Serializable]
public class AtackTrapData
{
    public float _tba;
    public int _damage;    

    public float _minDistance;
    public float _distanceToSlow;
}
