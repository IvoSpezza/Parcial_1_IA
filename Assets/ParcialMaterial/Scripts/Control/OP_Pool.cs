using System.Collections.Generic;
using UnityEngine;

public class OP_Pool
{
    
    private GameObject _objetToPool;
    private Transform _parent;
    private Queue<GameObject> _pool;
    private int _maxCount;
    private int _maxGenerated;

    public int _actives { get; private set; }

    public OP_Pool(GameObject objectToPoll,Transform parent, int maxGenerated)
    {
        _actives = 0;

        _maxGenerated = maxGenerated;
        _objetToPool = objectToPoll;
        _parent = parent;
        _maxCount = 0;
        _pool = new Queue<GameObject>();
    }

    public GameObject CreateObject()
    {
        _maxCount++;

        GameObject gameObject = Object.Instantiate(_objetToPool,_parent);

        gameObject.SetActive(false);

        _pool.Enqueue(gameObject);

        return gameObject;
    }

    public GameObject Get()
    {
        _actives++;
        
        if (_pool.Count == 0 && _maxCount < _maxGenerated)
        {
            CreateObject();            
        }

        GameObject gameObject = _pool.Dequeue();
        gameObject.SetActive(true);
        return gameObject;
    }

    public void Disable(GameObject gameObject)
    {
        _actives--;

        gameObject.SetActive(false);
        _pool.Enqueue(gameObject);
    }
        
}
