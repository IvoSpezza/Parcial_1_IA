using UnityEngine;

public class PathFinderVisual : MonoBehaviour
{
    private Node _startNode;
    private Node _endNode;      

    public void NodeClicked(Node node)
    {
        
        _startNode = HandleNodeClicked(node, _startNode);
        if (_startNode != null)
        {
            _startNode.ChangeColor(Color.green);
            if (_startNode == _endNode) _endNode = null;
        }
    }

    public void NodeRightClicked(Node node)
    {
        _endNode = HandleNodeClicked(node, _endNode);
        if (_endNode != null)
        {
            _endNode.ChangeColor(Color.red);
            if (_startNode == _endNode) _startNode = null;
        }
    }

    private Node HandleNodeClicked(Node node, Node currentNode)
    {
        if (currentNode != null)
        {
            currentNode.ChangeColor(Color.white);
            if (currentNode == node) return null;
        }
        return node;
    }

}
