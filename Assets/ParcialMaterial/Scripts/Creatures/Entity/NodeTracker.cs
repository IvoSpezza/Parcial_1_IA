using System;
using UnityEngine;

public class NodeTracker : MonoBehaviour
{
    [SerializeField] private LayerMask _nodeLayer;

    public event Action<Node> OnAnotherNode;
   
    private void OnTriggerEnter(Collider other)
    {        
        if(other.TryGetComponent<Node>(out Node node))
        {
        Node actualLocation = other.gameObject.GetComponent<Node>();
        OnAnotherNode?.Invoke(actualLocation);
        }
    }
}
