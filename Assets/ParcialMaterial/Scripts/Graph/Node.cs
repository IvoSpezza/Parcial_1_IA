using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    [Header("Masks")]
    [SerializeField] private LayerMask _hunterMask;
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private LayerMask _safeFloor;
    [SerializeField] private LayerMask _huntZone;
    [SerializeField] private LayerMask _startZone;
    [SerializeField] private LayerMask _endZone;

    [Header("Colors")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _blockedColor;
    [SerializeField] private Color _safeZoneColor;
    [SerializeField] private Color _huntZoneColor;
    [SerializeField] private Color _startColor;
    [SerializeField] private Color _endColor;


    public NodeType _type { get; private set; } = NodeType.Unnasigned;
    public void SetType(NodeType type) => _type = type;

    public event Action<Node> OnPlayerPosChange;
    public int Cost { get; private set; } = 1;

    public void SetCost(int cost) => Cost = cost;

    public Coordinates _coordinates { get; private set; }

    private Color _trueColor;
    public void SetCordenates(int x, int y) => _coordinates = new Coordinates(x, y);

    public event Action OnBlockedChange;

    private bool _blocked;
    private int _blocks = 0;

    public event Action<Node> OnNodeEndPreparation;
    public bool Blocked 
    {
        get => _blocked;

        private set 
        {
            _blocked = value;
            OnBlockedChange?.Invoke();
        } 
    }

    private void Awake()
    {
        _trueColor = _baseColor;
    }

    private void Start()
    {
        InitializeNode();
    }
    public void ChangeColor(Color color)
    {
        if (Blocked) return;
        _renderer.material.color = color;
    }

    public void Clean()
    {
        if (Blocked) return;
        _renderer.material.color = _trueColor;
    }
    
    private void OnValidate()
    {
        if(_renderer != null) return;
        if (!transform.GetChild(0).transform.GetChild(0).TryGetComponent(out _renderer)) Debug.LogError("this gameobject dosent have renderer component");
    }

    

    private void OnTriggerEnter(Collider other)
    {      

        if (GlobalFunc.IsValidTarget(other.gameObject.layer, _wallMask))
        {
            ChangeColor(_blockedColor);
            _blocks++;
            Blocked = true;            
        }         

        if (GlobalFunc.IsValidTarget(other.gameObject.layer, _hunterMask) && !Blocked) 
        {
            ChangeColor(Color.green);
            OnPlayerPosChange?.Invoke(this);
        }        
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(GlobalFunc.IsValidTarget(other.gameObject.layer, _hunterMask) && !Blocked)
        {
            Clean();
        }

        if (GlobalFunc.IsValidTarget(other.gameObject.layer, _wallMask))
        {
            _blocks--;
            if (_blocks == 0)
            {
                Blocked = false;
                Clean();    
            }
        }
    }

    public void InitializeNode()
    {
       
        float checkRadius = transform.localScale.y;
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius);
        
        foreach (var col in colliders)
        {
            int layer = col.gameObject.layer;
            
            if(GlobalFunc.IsValidTarget(layer, _wallMask) && !Blocked)
            {
                ChangeColor(_blockedColor);
                _blocks++;
                Blocked = true;
            }
            if (GlobalFunc.IsValidTarget(layer, _safeFloor)) { _type = NodeType.SafeZone; _trueColor = _safeZoneColor; }
            else if (GlobalFunc.IsValidTarget(layer, _huntZone)) { _type = NodeType.HuntField; _trueColor = _huntZoneColor; }
            else if (GlobalFunc.IsValidTarget(layer, _startZone)) { _type = NodeType.Start; _trueColor = _startColor; }
            else if (GlobalFunc.IsValidTarget(layer, _endZone)) { _type = NodeType.Finish; _trueColor = _endColor; }
        }

        if (_type == NodeType.Unnasigned)
        {
            _type = NodeType.FreeZone;
            _trueColor = _baseColor;
        }
        Clean();
        OnNodeEndPreparation?.Invoke(this);
    }
}

public struct Coordinates
{
    public int x { get; private set; }
    public int y { get; private set; }

    public Coordinates Up => new(x, y + 1);
    public Coordinates Down => new(x, y - 1);
    public Coordinates Left => new(x-1, y);
    public Coordinates Rigth => new(x+1, y);
    public Coordinates UpRigth => new(x+1, y+1);
    public Coordinates UpLeft => new(x - 1, y + 1);
    public Coordinates DownLedft => new(x - 1, y - 1);
    public Coordinates DownRigth => new(x + 1, y - 1);

    public List<Coordinates> GetAdyacent => new() { UpRigth, UpLeft, DownRigth, DownLedft, Up, Down, Left, Rigth };    
    public Coordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString() => $"({x},{y})";
}

public enum NodeType
{
    Unnasigned,
    FreeZone,
    SafeZone,
    HuntField,
    Finish,
    Start
}
