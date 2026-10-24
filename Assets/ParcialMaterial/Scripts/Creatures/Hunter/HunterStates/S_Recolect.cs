using System;
using System.Collections.Generic;
using UnityEngine;

public class S_Recolect : CreatureState
{
    private MS_Hunter _me;
    private RecolectData _data;        
    private HunterData _hunterData;
    private MS_BoidControlScript _deadBody;   
    private float _recolecting;

    public S_Recolect(RecolectData data, MS_Hunter me, StateMachine hunterMachine, HunterData hunterData,Animator animator)
    {
        _me = me;
        _data = data;
        _animator = animator;
        _stateMachine = hunterMachine;
        _hunterData = hunterData;
    }

    public override void Enter()
    {        
        _recolecting = 0;
        DetermineCorpose();
        _me.HunterDebDebugger.SetTitle("Recolecting", _hunterData._actualPoints+"", Color.deepPink);
        _me.HunterDebDebugger.selectedDebug("i see " + _hunterData._deadBoids.Count + " bodies", Color.deepPink);
    }
    private void DetermineCorpose()
    {
        if (_hunterData._deadBoids.Count == 0)
        {
            _deadBody = null;
            _stateMachine.ChangeState(HunterStates.Hunting);
            return;
        }

        _deadBody = _hunterData._deadBoids[0];
        float minDistance = (_deadBody.transform.position - _me.transform.position).sqrMagnitude;
        float checkDistance;
        foreach (MS_BoidControlScript closestCorpose in _hunterData._posiblePreys)
        {
            checkDistance = (closestCorpose.transform.position - _me.transform.position).sqrMagnitude;
            if (checkDistance < minDistance)
            {
                _deadBody = closestCorpose;
                minDistance = checkDistance;
            }
        }
    }

    public override void Update()
    {   
        _me.Arrive(_deadBody.transform.position, _data._minDistance, _data._dtss);              
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;

        if (_deadBody._isAlive)
        {
            _stateMachine.ChangeState(HunterStates.Hunting);
        }

        if (_me._velocity == Vector3.zero)
        {
            _me.transform.forward = (_deadBody.transform.position - _me.transform.position).normalized;            
            _animator.SetBool("Patrol", false);        
            _recolecting += Time.deltaTime;

            _me.HunterDebDebugger.selectedDebug("Recolecting in "+ (_data._timeToRecolect - _recolecting)+"s", Color.deepPink);

            if (_recolecting >= _data._timeToRecolect)
            {                
                _animator.SetTrigger("Recolect");
                _hunterData._actualPoints++;
                _deadBody.Recolect();

                if (_hunterData._actualPoints >= _data._poitnsToWin)
                {
                    _hunterData._maxPointsReached = true;
                    Debug.Log(_hunterData._maxPointsReached);
                    _stateMachine.ChangeState(HunterStates.Exit);
                }
                else
                {
                    _stateMachine.ChangeState(HunterStates.Hunting);
                }
            }
        }
    }
    public override void Exit()
    {
        _hunterData._deadBoids.Remove(_deadBody);
        
        _deadBody = null;
    }

    
}
[System.Serializable]
public class RecolectData
{
    [Header("Distance to start slowing")]
    public float _dtss;
    public float _minDistance;
    public float _timeToRecolect;
    public int _poitnsToWin;    
}