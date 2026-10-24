using UnityEngine;

public class MS_trapScript : MonoBehaviour
{
    private OP_Pool _myPool;
    [SerializeField] private int _life;

    private int _actualLife;
    public int Life => _actualLife;


    private void Awake()
    {
        _actualLife = _life;
    }
    public void SetPool(OP_Pool pool)
    {
        _myPool = pool;
    }

    public void Hit(int atackDmg)
    {
        _actualLife -= atackDmg;
        if (_actualLife <= 0)
        {            
            _myPool.Disable(this.gameObject);
        }
       
    }
    private void OnEnable()
    {
        _actualLife = _life;
    }
}
