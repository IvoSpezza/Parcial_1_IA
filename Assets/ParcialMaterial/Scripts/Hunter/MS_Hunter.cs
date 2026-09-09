using UnityEngine;

public class MS_Hunter : MS_Creature
{
    [SerializeField] private float _tba;
    [SerializeField] private float _rar;
    [SerializeField] private float _mar;

    
    private SphereCollider _viewDistance;

    private void Awake()
    {
        _viewDistance = GetComponent<SphereCollider>();

        _viewDistance.radius = _rar;
    }

}

public enum HunterStates
{
    Patrol,
    Recolect,
    Hunt
}
