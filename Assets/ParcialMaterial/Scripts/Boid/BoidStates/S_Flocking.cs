using System.Collections.Generic;
using UnityEngine;

public class S_Flocking : CreatureState
{
    private MS_BoidControlScript _me;    
    private FlockingData _flockingData;
    private BoidData _boidData;

    private Animator _animation;
    public S_Flocking(MS_BoidControlScript me,BoidData boidData, FlockingData flockingData,StateMachine stateMachine, Animator animation)
    {
        _me = me;
        _boidData = boidData;
        _stateMachine = stateMachine;
        _flockingData = flockingData;        
        _animation = animation;            
    }

    public override void Enter()
    {
        _me.CreatureDebugger.SetTitle("Flocking", _me.gameObject.name, Color.green);
        _animation.SetBool("IsSwiming", true);
    }

    public override void Update()
    {        
        Flocking();
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;

        if(CheckSurrondings())return;
        if(CheckTraps())return;
        _me.CreatureDebugger.selectedDebug("I have " + _boidData._nearAgents.Count + " friends\n " + "Speed: " + _me._velocity, Color.green);
    }


    public override void Exit()
    {
        _animation.SetBool("IsSwiming", false);
    }    

    private bool CheckSurrondings()
    {
        if(_boidData._hunter.TryPeek(out MS_Hunter hunter))
        {           
            _stateMachine.ChangeState(BoidState.Evading);
            return true;
        }
        return false;
    }
    private bool CheckTraps()
    {
        if (_boidData._nearbyTraps.Count > 0)
        {
            _stateMachine.ChangeState(BoidState.Atacking);
            return true;
        }
        return false;
    }


    private Vector3 CalculateAlignment()
    {
        Vector3 dessired = default;

        int deadOnes = 0;

        if (_boidData._nearAgents.Count == 0)
        {
            return Vector3.zero;
        }

        foreach (MS_BoidControlScript agent in _boidData._nearAgents)
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

        if (_boidData._nearAgents.Count - deadOnes == 0) return Vector3.zero;

        dessired /= _boidData._nearAgents.Count - deadOnes;
                
        return _me.CalculateSteering(dessired.normalized);
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 dessired = default;
        int deadOnes = 0;

        if (_boidData._nearAgents.Count == 0) return Vector3.zero;
      
        foreach (MS_BoidControlScript agent in _boidData._nearAgents)
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

        if (_boidData._nearAgents.Count - deadOnes == 0) return Vector3.zero;

        dessired /= (_boidData._nearAgents.Count -deadOnes);
               
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
    
