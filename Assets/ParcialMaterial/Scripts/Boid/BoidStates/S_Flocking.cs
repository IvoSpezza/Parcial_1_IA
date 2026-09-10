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
  
    }

    public override void Update()
    {
        Flocking();
    }

    public override void Exit()
    {
        
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

        Vector3 movement = aligment + cohesion;

        _me.AplyVelocity(movement);
    }


   

}

[System.Serializable]
public class FlockingData
{
    [SerializeField, Range(0, 6)] public float _separationWeight;
    [SerializeField, Range(0, 2)] public float _cohesionWeight ;
    [SerializeField, Range(0, 2)] public float _aligmentWeight ;
    public float _minEvadeDistance;
}
    
