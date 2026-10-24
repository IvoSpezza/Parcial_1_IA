using System;
using System.Collections.Generic;
using UnityEngine;


public class Graph : MonoBehaviour
{
    private const int COSTFORBLOCKEDNEIGHBORS = 6;

    [Header("Grid")]
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private float _nodeOffset = 1f;
    [SerializeField] private Node _nodePrefab;

    [Header("Toggle preview")]
    [SerializeField] private bool DRAW = true;

    private static Graph _instance;
    public static Graph Instance => _instance;

    private Node[,] _grid;

    public List<Node> _safeNodes = new();
    public List<Node> _huntZone { get; private set; } = new();
    public List<Node> _end = new();
    public Node HunterPos {  get; private set; }

    private int _repeated = 0;
    private HashSet<Node> _setNodes = new();

    public event Action OnGraphReady;
    
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        _grid = new Node[_gridWidth,_gridHeight];

        float scale = transform.localScale.x;

        for (int x = 0; x < _gridWidth; x++)
        {
            for(int y = 0; y < _gridHeight ; y++)
            {
                Node node = Instantiate(_nodePrefab, transform);
                node.SetCordenates(x,y);

                node.OnPlayerPosChange += SetPlayerPos;

                _grid[x, y] = node;
                node.transform.position = transform.position + new Vector3(x * _nodeOffset * scale, 0f, y * _nodeOffset * scale);
                node.name = $"Node({x},{y})";
                node.OnNodeEndPreparation += SetNode;
            }
        }                  
    }
    
    private void SetNode(Node node)
    {
        _repeated++;
        _setNodes.Add(node);

        if(_repeated != _gridHeight * _gridWidth) return;

        foreach (Node setNode in _setNodes)
        {
            setNode.OnNodeEndPreparation -= SetNode;
            if (setNode.Blocked)
            {
                foreach (Node neigbor in GetNeighbors(setNode))
                {
                    int actualCost = neigbor.Cost;
                    actualCost += COSTFORBLOCKEDNEIGHBORS;
                    neigbor.SetCost(actualCost);
                }
                continue;
            }
            
            switch (setNode._type)
            {
                case NodeType.SafeZone:
                    _safeNodes.Add(setNode);
                    break;
                case NodeType.Finish:
                    _end.Add(setNode);
                    break;
                case NodeType.HuntField:
                    _huntZone.Add(setNode);
                    break;
                default:                    
                    break;
            }                        
        }
        
        OnGraphReady?.Invoke();       
    }

    private void OnDisable()
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                _grid[x,y].OnPlayerPosChange -= SetPlayerPos;                
            }
        }
    }

    public List<Node> GetNeighbors(Node current)
    {
        List<Node> neighbors = new();

        Coordinates coordinates = current._coordinates;

        List<Coordinates> adjacentCords = coordinates.GetAdyacent;

        

        foreach (Coordinates cordin in adjacentCords)
        {
            if (!InsideGrid(cordin)) continue;

            Node neighbor = _grid[cordin.x, cordin.y];

            if (neighbor.Blocked)
            {                
                continue;
            }
            
            neighbors.Add(neighbor);
        }
        
        return neighbors;
    }

    public bool InsideGrid(Coordinates coordinates)
    {
        if (coordinates.x >= _gridWidth || coordinates.y >= _gridHeight) return false;
        if (coordinates.x < 0 || coordinates.y < 0) return false;
        return true;
    }

    private void SetPlayerPos(Node actualPos)
    {
        HunterPos = actualPos;
    }

    public List<Node> PathToObjective(Node start, Node end)
    {
        List<Node> path = PathFinder<Node>.AStarSearch(start,end, neighbor => neighbor.Blocked,current => GetNeighbors(current), 
                                                                nodeCost => nodeCost.Cost, nodeCords => nodeCords._coordinates );
        
        return path;
    }

    public List<Node> PathToObjective(Node start, Node end, NodeType notCount)
    {
        List<Node> path = PathFinder<Node>.AStarSearch(
            start,
            end,
            neighbor => neighbor.Blocked,
            current => GetNeighbors(current),
            nodeCost => nodeCost.Cost,
            nodeCords => nodeCords._coordinates,
            nodeType => nodeType._type,
            notCount
            );

        return path;
    }

    public Node GetClosestTypeNodeZone(Node actualNode, NodeType nodeType)
    {
        List<Node> whatList = nodeType switch
        {
            NodeType.SafeZone => _safeNodes,
            NodeType.Finish => _end,
            NodeType.HuntField => _huntZone,
            _ => null
        };

        Node closestNode = whatList[0];
        float distance = float.MaxValue;

        foreach(Node node in whatList)
        {
            distance = PathFinder<Node>.GetDistance(actualNode._coordinates, node._coordinates,false);
            if (distance < PathFinder<Node>.GetDistance(actualNode._coordinates, closestNode._coordinates,false))
            {
                closestNode = node;
            }
        }
        return closestNode;
    }
    public Node GetDeeperNodeInZone(Node start, NodeType zoneType, int minDepth = 2)
    {
        Queue<(Node node, int depth)> queue = new();
        HashSet<Node> visited = new();

        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, depth) = queue.Dequeue();

            if (depth >= minDepth && current._type == zoneType)
            {
                return current;
            }

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!visited.Add(neighbor))
                    continue;
                
                if (neighbor._type != zoneType)
                    continue;

                queue.Enqueue((neighbor, depth + 1));
            }
        }

        return start;
    }

    public Node GetRamdomNodeInType(NodeType nodeType)
    {
        List<Node> whatList = nodeType switch
        {
            NodeType.SafeZone => _safeNodes,
            NodeType.Finish => _end,
            NodeType.HuntField => _huntZone,
            _ => null
        };

        Node node = whatList[UnityEngine.Random.Range(0,whatList.Count)];
        return node;
    }

    public Node GetNearestNode(Vector3 worldPosition)
    {
        Node nearest = null;
        float minDistance = float.MaxValue;

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Node node = _grid[x, y];
                if (node == null) continue;

                float dist = Vector3.Distance(node.transform.position, worldPosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = node;
                }
            }
        }
        return nearest;
    }


    //-----Inspector size visualizer---------\\
    private void OnDrawGizmos()
    {
        if (!DRAW) return;
        Gizmos.color = Color.yellow;
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Gizmos.DrawCube(transform.position + new Vector3(x * _nodeOffset * transform.localScale.x, 0f, y * _nodeOffset * transform.localScale.z), transform.localScale);                
            }
        }
    }
}

public class PathInGraphData
{
    public Queue<Node> _actualPath = new();
    public Node _finalNode;
    public Node _nextNode;
    public Color _pathColor;
}
