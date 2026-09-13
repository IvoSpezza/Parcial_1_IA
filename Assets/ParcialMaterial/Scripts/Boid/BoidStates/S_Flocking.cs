using System.Collections.Generic;
using UnityEngine;

public class S_Flocking : CreatureState
{
    private MS_BoidControlScript _me;
    private List<MS_BoidControlScript> _agents;
    private FlockingData _flockingData;
    private Animator _animation;
    public S_Flocking(MS_BoidControlScript me, List<MS_BoidControlScript> agents, FlockingData flockingData, float maxSpeed,float steering, Animator animation)
    {
        _me = me;
        _agents = agents;
        _flockingData = flockingData;        
        _animation = animation;            
    }

    public override void Enter()
    {
        _animation.SetBool("IsSwiming", true);
    }

    public override void Update()
    {
        Flocking();
    }

    public override void Exit()
    {
        _animation.SetBool("IsSwiming", false);
    }    

    private Vector3 CalculateAlignment()
    {
        Vector3 dessired = default;

        int deadOnes = 0;

        if (_agents.Count == 0)
        {
            return Vector3.zero;
        }

        foreach (MS_BoidControlScript agent in _agents)
        {            
            if (agent._isAlive)
            {
                dessired += agent._velocity;
            }
            else
            {
                deadOnes++;
            }
        }

        if (_agents.Count - deadOnes == 0) return Vector3.zero;

        dessired /= _agents.Count - deadOnes;
                
        return _me.CalculateSteering(dessired.normalized);
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 dessired = default;
        int deadOnes = 0;

        if (_agents.Count == 0) return Vector3.zero;
      
        foreach (MS_BoidControlScript agent in _agents)
        {
            if (agent._isAlive)
            {
                dessired += agent.transform.position;
            }
            else
            {
                deadOnes++;
            }
        }

        if (_agents.Count - deadOnes == 0) return Vector3.zero;

        dessired /= (_agents.Count-deadOnes);
               
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
    
