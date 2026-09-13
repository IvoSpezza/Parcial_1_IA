using System.Collections.Generic;
using UnityEngine;

public class OP_Pool
{
    private int _maxCount;
    private GameObject _objetToPool;
    private Transform _parent;
    private Queue<GameObject> _pool;
    private int _maxGenerated;

    public OP_Pool(GameObject objectToPoll,Transform parent, int maxGenerated)
    {
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
        gameObject.SetActive(false);
        _pool.Enqueue(gameObject);
    }
        
}
