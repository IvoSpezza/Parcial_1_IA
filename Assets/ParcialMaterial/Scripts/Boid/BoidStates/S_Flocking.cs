using System.Collections.Generic;
using UnityEngine;

public class S_Flocking : CreatureState
{
    private MS_BoidControlScript _me;
    private List<MS_BoidControlScript> _agents;
    private FlockingData _flockingData;
    private float _steering;
    public S_Flocking(MS_BoidControlScript me, List<MS_BoidControlScript> agents, FlockingData flockingData, float maxSpeed,float steering)
    {
        _me = me;
        _agents = agents;
        _flockingData = flockingData;
        _maxSpeed = maxSpeed;
        _steering = steering;
    }

    public override void Enter()
    {
        RandomMovement();
        _me.AnimationHasToChange(BoidState.Flocking);
    }

    public override void Update()
    {
        Flocking();
        _me.transform.position += _velocity * Time.deltaTime;
        _me.transform.forward = _velocity;
        _me.SetVelocity(_velocity);
    }

    public override void Exit()
    {
        
    }

    private void RandomMovement()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1, 1), 0f, Random.Range(-1, 1));
        _velocity = randomDirection.normalized * _maxSpeed;
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 dessired = default;
        if (_agents.Count == 0)
        {
            return dessired;
        }

        foreach (MS_BoidControlScript agent in _agents)
        {
            if (Vector3.Distance(agent.transform.position, _me.transform.position) <= _flockingData._minEvadeDistance)
            {
                dessired += _me.transform.position - agent.transform.position;
            }
        }
        dessired.y = 0;
        if (dessired.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }
        return CalculateSteering(dessired.normalized, _steering);
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 dessired = default;
        if (_agents.Count == 0)
        {
            return dessired;
        }
        foreach (MS_BoidControlScript agent in _agents)
        {
            dessired += agent._velocity;
        }
        dessired /= _agents.Count;
        dessired.y = 0;
        if (dessired.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }
        return _me.CalculateSteering(dessired.normalized);
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 dessired = default;
        if (_agents.Count == 0)
        {
            return dessired;
        }
        foreach (MS_BoidControlScript agent in _agents)
        {
            dessired += agent.transform.position;
        }

        dessired /= _agents.Count;

        dessired -= _me.transform.position;
        dessired.y = 0;
        if (dessired.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }
        return _me.CalculateSteering(dessired.normalized);
    }

    private void Flocking()
    {

        Vector3 aligment = CalculateAlignment() * _flockingData._aligmentWeight; 
        Vector3 cohesion = CalculateCohesion() * _flockingData._cohesionWeight;
        Vector3 separation = CalculateSeparation() * _flockingData._separationWeight;

        Vector3 movement = aligment + cohesion + separation;

        _velocity += movement;
    }


   

}

[System.Serializable]
public class FlockingData
{
    [SerializeField, Range(0,2)] public float _separationWeight;
    [SerializeField, Range(0, 2)] public float _cohesionWeight ;
    [SerializeField, Range(0, 2)] public float _aligmentWeight ;
    public float _minEvadeDistance;
}
    
