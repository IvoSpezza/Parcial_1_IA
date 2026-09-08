using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MS_BoidControlScript : MS_Creature
{
    [SerializeField] private float _maxSeeDistance;
    
    [SerializeField] private FlockingData _dataForFlocking;

    private StateMachine _machine;
    private List<MS_BoidControlScript> _agents;

    private SphereCollider _collision;

    public event Action<BoidState> OnChangeState;

    private void Awake()
    {
        _collision = GetComponent<SphereCollider>();

        _collision.radius = _maxSeeDistance;

        _machine = new StateMachine();

        _agents = new List<MS_BoidControlScript>();

        S_Flocking flocking = new S_Flocking(this, _agents, _dataForFlocking, _maxSpeed, _steering);

        _machine.AddState(flocking, BoidState.Flocking);

        _machine.ChangeState(BoidState.Flocking);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        _machine.MachineUpdate();
        transform.position = Bounds.instance.OutOfBounds(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript anotherAgent))
        {
          
            _agents.Add(anotherAgent);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript anotherAgent))
        {
            _agents.Remove(anotherAgent);
            
        }
    }

    public void AnimationHasToChange(BoidState newState)
    {
        OnChangeState?.Invoke(newState);
    }

    public void SetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
    }

}

public enum BoidState
{
    Flocking,
    Evading,
    Atacking,
    Dead
}
