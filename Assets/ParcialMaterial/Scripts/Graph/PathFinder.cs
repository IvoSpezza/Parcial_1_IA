using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PathFinder<T> 
{
    public static List<T> BRS(T start, T end, Func<T, bool> isBlocked, Func<T, List<T>> getNeighbors)
    {
        Queue<T> analize = new();
        analize.Enqueue(start);

        Dictionary<T, T> cameFrom = new();
        cameFrom.Add(start, default);

        HashSet<T> procesed = new();

        T current;

        while (analize.Count != 0)
        {
            current = analize.Dequeue();
            if (isBlocked(current)) continue;

            if (!procesed.Add(current)) continue;

            if (current.Equals(end)) return ReconstructPath(cameFrom, current);

            foreach (T neighbor in getNeighbors(current))
            {
                if (isBlocked(neighbor))
                {
                    Debug.Log("BLOQUEADo");
                    continue;
                }              
                if (cameFrom.ContainsKey(neighbor)) continue;
                analize.Enqueue(neighbor);
                cameFrom[neighbor] = current;                               
            }
        }
        return new();
    }
    public static List<T> AStarSearch(T start, T end, Func<T,bool> isBlocked, Func<T,List<T>> getNeighbors, Func<T,int> getCost, Func<T,Coordinates> getCordinates)
    {
        if (start == null || end == null) return new();

        PriorityQueue<T> frontier = new();
        frontier.Enqueue(start,0);

        Dictionary<T, int> costSoFar = new();
        costSoFar.Add(start, 0); 

        Dictionary<T, T> cameFrom = new();
        cameFrom.Add(start, default);

        HashSet<T> procesed = new(); 

        T current;
        while(frontier.Count != 0)
        {
            current = frontier.Dequeue();
            if (isBlocked(current)) continue;

            if (!procesed.Add(current)) continue;

            if (current.Equals(end)) return ReconstructPath(cameFrom, current);

            foreach (T neighbor in getNeighbors(current))
            {
                if (isBlocked(neighbor))
                {
                    Debug.Log("BLOQUEADo"); 
                    continue; 
                }              

                int newCost = costSoFar[current] + getCost(neighbor);
                if (cameFrom.ContainsKey(neighbor) && newCost >= costSoFar[neighbor]) continue;

                frontier.Enqueue(neighbor, newCost + GetDistance(getCordinates(neighbor),getCordinates(end),true));                
                cameFrom[neighbor] = current;
                costSoFar[neighbor] = newCost;
                procesed.Remove(neighbor);
            }
        }
        return new();
    }

    public static List<T> AStarSearch(T start, T end, Func<T, bool> isBlocked, Func<T, List<T>> getNeighbors, Func<T, int> getCost, Func<T, Coordinates> getCordinates, Func<T,Enum> discriminant, Enum type)
    {
        if (start == null || end == null) return new();

        PriorityQueue<T> frontier = new();
        frontier.Enqueue(start, 0);

        Dictionary<T, int> costSoFar = new();
        costSoFar.Add(start, 0);

        Dictionary<T, T> cameFrom = new();
        cameFrom.Add(start, default);

        HashSet<T> procesed = new();

        T current;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (isBlocked(current)) continue;


            if (!procesed.Add(current)) continue;

            if (current.Equals(end)) return ReconstructPath(cameFrom, current);

            foreach (T neighbor in getNeighbors(current))
            {
                if (isBlocked(neighbor))
                {
                    Debug.Log("BLOQUEADo");
                    continue;
                }

                if (discriminant(current).Equals(type)) continue;

                int newCost = costSoFar[current] + getCost(neighbor);
                if (cameFrom.ContainsKey(neighbor) && newCost >= costSoFar[neighbor]) continue;

                frontier.Enqueue(neighbor, newCost + GetDistance(getCordinates(neighbor), getCordinates(end), true));
                cameFrom[neighbor] = current;
                costSoFar[neighbor] = newCost;
                procesed.Remove(neighbor);
            }
        }
        return new();
    }
    public static float GetDistance(Coordinates a, Coordinates b,bool exact)
    {
       
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        float distance = dx * dx +  dy * dy ;

        if(exact) distance = Mathf.Sqrt(distance);
        return distance;
    }

    private static List<T> ReconstructPath(Dictionary<T,T> cameFrom, T end)
    {
        List<T> path = new();

        while(end != null)
        {
            path.Add(end);
            end = cameFrom[end];
        }

        path.Reverse();

        return path;
    }
}

public class PathsManager
{
    private PathData _data;
    private NodeType _discriminant;

    public PathsManager(PathData data, Node actualNode, NodeType discriminant = NodeType.Unnasigned)
    {
        _data = data;
        _data._objetive = actualNode;
        _discriminant = discriminant;
    }
    
    public bool SetStartAndEnd(Node start,Node end)
    {        
        if (end == null) return false;

        if (!_discriminant.Equals(NodeType.Unnasigned) && end._type.Equals(_discriminant)) return false;

        _data._actualPointOfinteres = end;
        _data._objetive = start;
        RefreshPath();
        return true;
    }

    public bool Next()
    {
        if (_data._actualPath.Count == 0) return true;
       
        _data._objetive.Clean();
        _data._objetive.OnBlockedChange -= RefreshPath;
        _data._objetive = _data._actualPath.Dequeue();
        
        return false;
    }

    private void SetPath(List<Node> nodesToTravel)
    {
        if (nodesToTravel == null || nodesToTravel.Count == 0)
        {
            Debug.LogWarning("Seth path failed, nodes to trabel null or empty");
            return;
        }

        foreach (Node node in nodesToTravel)
        {
            node.ChangeColor(_data._pathColor);
            _data._actualPath.Enqueue(node);
            node.OnBlockedChange += RefreshPath;
        }
    }

    private void RefreshPath()
    {
        DeletePath();

        if (_data._objetive == null || _data._actualPointOfinteres == null) return;
        
        if (_discriminant.Equals(NodeType.Unnasigned))
        {
            SetPath(Graph.Instance.PathToObjective(_data._objetive, _data._actualPointOfinteres));
        }
        else
        {
            SetPath(Graph.Instance.PathToObjective(_data._objetive, _data._actualPointOfinteres, _discriminant));
        }
    }

    public void DeletePath()
    {
        while (_data._actualPath.Count > 0)
        {
            Node remove = _data._actualPath.Dequeue();
            remove.OnBlockedChange -= RefreshPath;
            remove.Clean();
        }
    }

    public Node Actual() => _data._objetive;

    public Vector3 Desired() => _data._objetive != null ? _data._objetive.transform.position : Vector3.zero;
    
    public bool HasReachedEnd() => _data._actualPath.Count == 0;
}

public class PathData
{
    public PathData(Color pathColor)
    {
        _pathColor = pathColor;
    }

    public Queue<Node> _actualPath = new();
    public Node _actualPointOfinteres;
    public Node _objetive;
    public Color _pathColor { get; private set; }
}