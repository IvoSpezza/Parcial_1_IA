using System;
using System.Collections.Generic;
using UnityEngine;

public class S_Recolect : CreatureState
{
    private MS_Hunter _me;
    private RecolectData _data;
    private Animator _animator;
    private StateMachine _hunterMachine;
    private HunterData _hunterData;
    private MS_BoidControlScript _deadBody;
    private float _recolecting;

    public S_Recolect(RecolectData data, MS_Hunter me, StateMachine hunterMachine, HunterData hunterData,Animator animator)
    {
        _me = me;
        _data = data;
        _animator = animator;
        _hunterMachine = hunterMachine;
        _hunterData = hunterData;
    }

    public override void Enter()
    {
        Debug.Log("ENTRO A RECOLECT");
        _recolecting = 0;
        DetermineCorpose();
    }
    private void DetermineCorpose()
    {
        if (_hunterData._deadBoids.Count == 0)
        {
            _deadBody = null;            
            return;
        }

        _deadBody = _hunterData._deadBoids[0];
        float minDistance = Vector3.Distance(_me.transform.position, _deadBody.transform.position);
        float checkDistance;
        foreach (MS_BoidControlScript closestCorpose in _hunterData._posiblePreys)
        {
            checkDistance = Vector3.Distance(_me.transform.position, closestCorpose.transform.position);
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

        if (_me._velocity == Vector3.zero)
        {
            _me.transform.forward = (_deadBody.transform.position - _me.transform.position).normalized;
            
            _animator.SetBool("Patrol", false);
            
            _recolecting += Time.deltaTime;

            if(_recolecting >= _data._timeToRecolect)
            {                
                _animator.SetTrigger("Recolect");

                _deadBody.Recolect();                            
                _hunterMachine.ChangeState(HunterStates.Patrol);
            }

        }
    }
    public override void Exit()
    {
        _hunterData._deadBoids.Remove(_deadBody);
        Debug.Log("SALIO DE RECOLECT");
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
}