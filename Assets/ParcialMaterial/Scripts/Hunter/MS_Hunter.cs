using UnityEngine;

public class MS_Hunter : MS_Creature
{
    [SerializeField] private float _tba;
    [SerializeField] private float _rar;
    [SerializeField] private float _mar;

    [SerializeField] private PatrolData _patrol;

    private StateMachine _hunterMachine;

    
    private SphereCollider _viewDistance;

    private void Awake()
    {
        _viewDistance = GetComponent<SphereCollider>();
        _viewDistance.radius = _rar;

        _hunterMachine = new StateMachine();

        S_Patrol patrolState = new S_Patrol(_patrol,this);

        _hunterMachine.AddState(patrolState, HunterStates.Patrol);
        _hunterMachine.ChangeState(HunterStates.Patrol);
    }

    private void Update()
    {
        _hunterMachine.MachineUpdate();
    }

}

public enum HunterStates
{
    Patrol,
    Recolect,
    Hunt
}
