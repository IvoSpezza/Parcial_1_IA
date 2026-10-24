using NUnit.Framework;

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
    

    protected bool InViewAngle(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, dirToTarget) <= _viewAngle * 0.5f;
    }

    protected bool HasLineOfSight(Transform target)
    {
        Vector3 eyeHeigt = Vector3.up * _eyeHeight;
        Vector3 eyepos = transform.position + eyeHeigt;

        Vector3 extactPos = target.position;
        Vector3 eyeHeigtPos = target.position + eyeHeigt;

        Vector3 exactDirection = extactPos - eyepos;
        Vector3 heigthDirection = eyeHeigtPos - eyepos;

        if (TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
            Debug.DrawLine(eyepos, extactPos, Color.red, 0.1f);
        }            

        if(Physics.Raycast(eyepos,exactDirection.normalized,out RaycastHit hit, exactDirection.magnitude, _obstacleMask))           
        {
            return hit.transform == target || hit.transform.IsChildOf(target);
        }
        if(Physics.Raycast(eyepos, eyeHeigtPos.normalized, out RaycastHit hHit, exactDirection.magnitude, _obstacleMask))
        {
            return hHit.transform == target || hHit.transform.IsChildOf(target);
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!GlobalFunc.IsValidTarget(other.gameObject.layer,_targetMask)) return;
        ValidTargetIn(other);                
    }

    private void OnTriggerExit(Collider other)
    {
        if (!GlobalFunc.IsValidTarget(other.gameObject.layer, _targetMask)) return;         
        ValidTargetOut(other);
    }

    protected virtual void ValidTargetIn(Collider target){}

    protected virtual void ValidTargetOut(Collider target){}
    
    
}

public static class GlobalFunc
{
   public static bool IsValidTarget(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0; 
}