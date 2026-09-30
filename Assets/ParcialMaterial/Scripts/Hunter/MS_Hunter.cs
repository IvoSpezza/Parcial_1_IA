using System.Collections.Generic;
using UnityEngine;

public class MS_Hunter : MS_Creature
{    
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private int _bulletsAmount;
    [SerializeField] private GameObject _bulletPrefab;
    private OP_Pool _bulletsPool;

    [Header("States Data")]
    [SerializeField] private PatrolData _patrol;
    [SerializeField] private AtackDatta _atack;
    [SerializeField] private RecolectData _recolectData;

    [SerializeField] public MS_CreatureDebugger HunterDebDebugger;

    private FieldOfView _fieldOfView;

    private StateMachine _hunterMachine;
    public HunterData _hunterData { get; private set; }

    private void Awake()
    {
        _fieldOfView = GetComponent<FieldOfView>();

        Animator animation = GetComponent<Animator>();

        _hunterMachine = new StateMachine();

        _hunterData = new HunterData();
        _hunterData._posiblePreys = new List<MS_BoidControlScript>();
        _hunterData._deadBoids = new List<MS_BoidControlScript>();
        _hunterData._canAtack = false;

        _bulletsPool = new OP_Pool(_bulletPrefab, transform.parent, _bulletsAmount);

        S_Patrol patrolState = new S_Patrol(_patrol, this, _hunterMachine, _hunterData, animation);
        _hunterMachine.AddState(patrolState, HunterStates.Patrol);

        S_Atack atacState = new S_Atack(_atack, this, _hunterMachine, _hunterData, animation);
        _hunterMachine.AddState(atacState, HunterStates.Hunt);

        S_Recolect recolect = new S_Recolect(_recolectData,this,_hunterMachine,_hunterData, animation);        
        _hunterMachine.AddState(recolect, HunterStates.Recolect);

        _hunterMachine.ChangeState(HunterStates.Patrol);
    }

    private void Start()
    {
        MS_Bullet bullet;
        for(int i = 0;i<_bulletsAmount ; i++)
        {
            bullet = _bulletsPool.CreateObject().GetComponent<MS_Bullet>();
            bullet.SetPool(_bulletsPool);
        }
    }

    private void Update()
    {
        AvoidObstacles(_fieldOfView.EyeHeight, _fieldOfView.ObstacleMask);
        _hunterMachine.MachineUpdate();
        transform.position = Bounds.instance.OutOfBounds(transform.position);
    }      

    public void Shoot(Vector3 objetibe)
    {
        GameObject bullet = _bulletsPool.Get();
        bullet.transform.position = _shootingPoint.position;
        bullet.transform.forward = (objetibe - bullet.transform.position).normalized;
    }

}
public class HunterData
{
    public List<MS_BoidControlScript> _posiblePreys;
    public List<MS_BoidControlScript> _deadBoids;
    public bool _canAtack;
}

public enum HunterStates
{
    Patrol,
    Recolect,
    Hunt,
    Atack
}
