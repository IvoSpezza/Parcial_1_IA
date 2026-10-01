using UnityEngine;

public class Graph : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private float _nodeOffset = 1f;
    [SerializeField] private Node _nodePrefab;

    
    private Node[,] _grid;
    private PathFinderVisual _pathFinder;

    private void Awake()
    {
        _pathFinder = GetComponent<PathFinderVisual>();
    }
    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        _grid = new Node[_gridWidth,_gridHeight];

        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for(int y = 0; y < _grid.GetLength(1); y++)
            {
                Node node = Instantiate(_nodePrefab, transform);

                node.OnNodeClicked += _pathFinder.NodeClicked;
                node.OnNodeRigthClicked += _pathFinder.NodeRightClicked;

                _grid[x, y] = node;
                node.transform.position = new(x * _nodeOffset, 0f, y * _nodeOffset);
                node.name = $"Node({x},{y})";
            }
        }
    }

    // asi te gusta profe?
    private void OnDisable()
    {
        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                _grid[x,y].OnNodeClicked -= _pathFinder.NodeClicked;
                _grid[x,y].OnNodeRigthClicked -= _pathFinder.NodeRightClicked;                
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Gizmos.DrawCube(new(x * _nodeOffset * transform.localScale.x, 0f, y * _nodeOffset * transform.localScale.z), transform.localScale);                
            }
        }
    }
}
