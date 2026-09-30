using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    [Header("FieldOfView")]
    [SerializeField] private float _viewRadius = 0f;
    [SerializeField] private float _eyeHeight = 1f;
    [SerializeField, UnityEngine.Range(0f, 360f)] private float _viewAngle = 100f;

    [Header("Filters")]
    [SerializeField] protected LayerMask _targetMask;
    [SerializeField] protected LayerMask _obstacleMask;

    public LayerMask TargetMask => _targetMask;
    public LayerMask ObstacleMask => _obstacleMask;

    private void Awake()
    {
        SphereCollider colision = GetComponent<SphereCollider>();
        colision.radius = _viewRadius;
    }

    public bool CanSeeTarget(Transform target) => InViewAngle(target) && HasLineOfSight(target);  
    
    public float ViewRadius => _viewRadius;
    public float EyeHeight => _eyeHeight;
    public float ViewAngle => _viewAngle;
      
    protected bool IsValidTarget(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0; 

    protected bool InViewAngle(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, dirToTarget) <= _viewAngle * 0.5f;
    }

    protected bool HasLineOfSight(Transform target)
    {
        Vector3 eyepos = transform.position + Vector3.up * _eyeHeight;
        Vector3 endOfSigth = target.position;
        endOfSigth.y = eyepos.y;
        Vector3 direction = endOfSigth - eyepos;

        if (TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
            Debug.DrawLine(eyepos, endOfSigth, Color.blue, 0.1f);
        }
            

        if(Physics.Raycast(eyepos,direction.normalized,out RaycastHit hit, direction.magnitude, _obstacleMask))
        {
            return hit.transform == target || hit.transform.IsChildOf(target);
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsValidTarget(other.gameObject.layer,_targetMask)) return;
        ValidTargetIn(other);                
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidTarget(other.gameObject.layer, _targetMask)) return;         
        ValidTargetOut(other);
    }

    protected virtual void ValidTargetIn(Collider target){}

    protected virtual void ValidTargetOut(Collider target){}
    
    
}
