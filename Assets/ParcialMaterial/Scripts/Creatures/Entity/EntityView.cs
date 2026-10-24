using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.GraphicsBuffer;

public class EntityView : FieldOfView
{    
    private MS_Entity _me;
    private void Start()
    {
        _me = GetComponent<MS_Entity>();       
        EntityData.EyesOnHunter = new();
    }
    protected override void ValidTargetIn(Collider target)
    {       
        
        if (!CanSeeTarget(target.transform) || ValidPos()) return;

        if (target.transform.root.gameObject.TryGetComponent<MS_Hunter>(out MS_Hunter hunter))
        {
           _me._entityData.Hunter = hunter;
            EntityData.EyesOnHunter.Add(_me);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!GlobalFunc.IsValidTarget(other.gameObject.layer, _targetMask)) return;

        if (CanSeeTarget(other.transform) && ValidPos())
        {
            if (other.transform.root.gameObject.TryGetComponent<MS_Hunter>(out MS_Hunter hunter))
            {
                _me._entityData.Hunter = hunter;
                EntityData.EyesOnHunter.Add(_me);
            }
        }
        else
        {
            EntityData.EyesOnHunter.Remove(_me);
            _me._entityData.Hunter = null;
        }
    }

    private bool ValidPos()
    {
        return !Graph.Instance.HunterPos._type.Equals(NodeType.SafeZone) && !Graph.Instance.HunterPos._type.Equals(NodeType.Start);
    }

    protected override void ValidTargetOut(Collider target)
    {

        EntityData.EyesOnHunter.Remove(_me);
        _me._entityData.Hunter = null;

    }
}
