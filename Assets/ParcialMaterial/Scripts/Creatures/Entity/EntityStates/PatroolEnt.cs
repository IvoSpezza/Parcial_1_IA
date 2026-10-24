using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Patrool : CreatureState
{
    private Graph _graph = Graph.Instance;

    private MS_Entity _me;
    private EntityPatrolWayPoints _data;
    private StateMachine _machine;
    private PathsManager _pathManager;
    private EntityData _meData;

    private Cicle<Node> _pointsOfInterest;

    

    public Patrool(EntityData meData,MS_Entity me, EntityPatrolWayPoints data, StateMachine machine, PathsManager pathManager)
    {
        _me = me;
        _data = data;
        _machine = machine;
        _pathManager = pathManager;
        _meData = meData;
        List<Node> pathPoints = new();

        if (_graph == null) Debug.LogError("¡_graph es null en Patrool!");
        if (_data == null) Debug.LogError("¡_data es null en Patrool!");
        if (_data.Points == null) Debug.LogError("¡_data.Points array es null en Patrool!");

        foreach (Transform point in _data.Points)
        {
            Node node = _graph.GetNearestNode(point.position);                        
            pathPoints.Add(node);
        }
        _pointsOfInterest = new Cicle<Node>(pathPoints);       
    }

    public override void Enter()
    {
        Node next = _pointsOfInterest.Next();
        _pathManager.SetStartAndEnd(_me._miNodePos,next);
        _me._debugger.SetTitle("Patrool",_me.gameObject.name,Color.green);
        
    }


    public override void Update()
    {        
        Patrol();
        _me.transform.position += _me._velocity * Time.deltaTime;
        _me.transform.forward = _me._velocity;

        _me._debugger.selectedDebug($"Actual destini = {_pointsOfInterest.Current}",Color.greenYellow);

        if(_meData.Hunter != null) // siloveo
        {
            _machine.ChangeState(EntityState.Follow);
        }
        if(EntityData.EyesOnHunter.Count  > 0) // sialguienlove
        {
            _machine.ChangeState(EntityState.Hunt);
        }
    }

    public override void Exit()
    {
        _pathManager.DeletePath();
    }

    private void Patrol()
    {
        float distance = Vector3.Distance(_me.transform.position, _pathManager.Desired());
        
        if(distance <= _data.MinDistanceToNode)
        {            
            if (_pathManager.Next())
            {                
                _pathManager.SetStartAndEnd(_me._miNodePos,_pointsOfInterest.Next());                       
            }
        }
        _me.CustomSpeedSeek(_pathManager.Desired(),_data.PatrolSpeed);
    }
    
}

[System.Serializable]
public class EntityPatrolWayPoints
{
    public Transform[] Points;

    public float PatrolSpeed;

    public LayerMask NodeMask;

    public float MinDistanceToNode;
}