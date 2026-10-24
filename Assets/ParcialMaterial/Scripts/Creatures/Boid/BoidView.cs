using UnityEngine;

public class BoidView : FieldOfView
{
    
    private BoidData _boidData;
    void Start()
    {
        MS_BoidControlScript meAsBoid = GetComponent<MS_BoidControlScript>();

        _boidData = meAsBoid._dataForBoid;
    }

    protected override void ValidTargetIn(Collider target)
    {
        if(!CanSeeTarget(target.transform)) return;

        GameObject hunter = target.transform.root.gameObject;
        
        if (hunter.TryGetComponent<MS_Hunter>(out MS_Hunter enemy))
        {
            
            _boidData._hunter.Push(enemy);            
            return;
        }

        if (hunter.TryGetComponent<MS_trapScript>(out MS_trapScript trap))
        {

            _boidData._nearbyTraps.Add(trap);
            return;
        }

    }
    protected override void ValidTargetOut(Collider target)
    {
        GameObject trap = target.gameObject;
        if (trap.TryGetComponent<MS_trapScript>(out MS_trapScript theTrap))
        {
            _boidData._nearbyTraps.Remove(theTrap);
        }

    }
}
