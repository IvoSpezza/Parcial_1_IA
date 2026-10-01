using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public event Action<Node> OnNodeClicked = delegate { };
    public event Action<Node> OnNodeRigthClicked = delegate { };

    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject _wallObject;

    public bool Blocked 
    {
        get => _blocked;
        private set
        {
            _blocked = value;
            _wallObject.SetActive(value);
        }
    }
    public bool _blocked;

    private void OnMouseOver()
    {        
        if (Input.GetMouseButtonDown(0)) OnNodeClicked?.Invoke(this);
        else if (Input.GetMouseButtonDown(1)) OnNodeRigthClicked?.Invoke(this);
        else if (Input.GetMouseButtonDown(2)) Blocked = !_blocked;
        else if (Input.GetMouseButton(2)) Blocked = false;
    }

    public void ChangeColor(Color color) => _renderer.material.color = color;

    private void OnValidate()
    {
        if(_renderer != null) return;
        if (!transform.GetChild(0).transform.GetChild(0).TryGetComponent(out _renderer)) Debug.LogError("this gameobject dosent have renderer component");
    }
}
