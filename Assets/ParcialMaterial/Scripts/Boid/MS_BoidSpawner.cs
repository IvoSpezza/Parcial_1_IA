using System.Collections.Generic;
using UnityEngine;

public class MS_BoidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _boid;
    [SerializeField] private int _boidsGenerated = 6;
    [SerializeField] private float _timeToRespawn;

    private OP_Pool _pool;
    private float _time;
    private int _reviveQueue;

    private void Awake()
    {
        _reviveQueue = 0;
        _pool = new OP_Pool(_boid,transform, _boidsGenerated);
        _time = 0;
    }

    private void Start()
    {
        MS_BoidControlScript boid;
        for (int i = 0; i < _boidsGenerated; i++)
        {
            boid = _pool.Get().GetComponent<MS_BoidControlScript>(); ;
            boid.OnBodyRecolected += QueueRevive;
            boid.transform.position = GetRandomPosition();
        }
    }

    private void Update()
    {
        if(_reviveQueue > 0)_time += Time.deltaTime;

        if(_time >= _timeToRespawn)
        {
            _time = 0;
            Revive(GetRandomPosition());
        }
    }

    private Vector3 GetRandomPosition()
    {
        float randX = Random.Range(-Bounds.instance.Width/2, (Bounds.instance.Width / 2) +1);
        float randZ = Random.Range(-Bounds.instance.Heigth / 2, (Bounds.instance.Heigth / 2) + 1);

        return new Vector3(randX, 0, randZ);
    }

    private void Revive(Vector3 newPos) 
    {
        _pool.Get().transform.position = newPos;
        _reviveQueue--;
    }
    private void QueueRevive() => _reviveQueue++;
}
