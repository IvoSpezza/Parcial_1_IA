using System;
using System.Collections.Generic;
using UnityEngine;

public class S_Recolect : CreatureState
{
    private MS_Hunter _me;
    private RecolectData _data;
    private Animator _animator;
    private Stack<MS_BoidControlScript> _deadBoids;
    private MS_BoidControlScript _deadBody;
    private float _recolecting;

    public event Action<bool> OnRecolect;

    public S_Recolect(MS_Hunter me, RecolectData data,Stack<MS_BoidControlScript> deadBoids,Animator animator)
    {
        _me = me;
        _data = data;
        _animator = animator;
        _deadBoids = deadBoids;
    }

    public override void Enter()
    {
        _recolecting = 0;
        _deadBody = _deadBoids.Pop();
    }

    public override void Update()
    {
        Vector3 steering = _me.Arrive(_deadBody.transform.position, _data._minDistance, _data._dtss);
        _me.AplyVelocity(steering);        
        _me.transform.position += steering;
        _me.transform.forward = steering;

        if (steering == Vector3.zero)
        {
            _recolecting += Time.deltaTime;

            if(_recolecting >= _data._timeToRecolect)
            {
                _animator.SetTrigger("Recolect");
                OnRecolect?.Invoke(false);
                _deadBody.Recolect();
            }

        }
    }
    public override void Exit()
    {
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