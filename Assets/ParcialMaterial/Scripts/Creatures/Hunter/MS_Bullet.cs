using UnityEngine;

public class MS_Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _timeAlive;

    private float _time;
    public Vector3 _velocity {  get; private set; }

    private OP_Pool _pool;

    private void Awake()
    {
        _time = 0;
    }

    public void SetPool(OP_Pool pool)
    {
        _pool = pool;
    }
    
    void Update()
    {
        transform.position += (transform.forward *_speed) * Time.deltaTime;
        _time += Time.deltaTime;
        if (_time >= _timeAlive) _pool.Disable(this.gameObject);
    }

    private void OnEnable()
    {
        _time = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {       
        if(collision.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
            prey.Die();            
            _pool.Disable(this.gameObject);
        }
    }
}
