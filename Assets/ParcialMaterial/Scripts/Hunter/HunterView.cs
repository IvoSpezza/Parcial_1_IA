using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HunterView : FieldOfView 
{
    private HunterData _hunterData;
    private void Start()
    {
        _hunterData = GetComponent<MS_Hunter>()._hunterData;
    }
    protected override void ValidTargetIn(Collider target)
    {
       
        GameObject boid = target.gameObject.transform.parent.gameObject;

        bool iCanSeeBoid = CanSeeTarget(target.transform);

        if (boid.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey) && iCanSeeBoid)
        {

            if (!prey._isAlive)
            {
                _hunterData._deadBoids.Add(prey);
                return;
            }           
            _hunterData._posiblePreys.Add(prey);
          
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsValidTarget(other.gameObject.layer, _targetMask)) return;

        GameObject boid = other.gameObject.transform.parent.gameObject;

        
        if (boid.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
                
            if (CanSeeTarget(other.transform) && !CheckIsIsAlreadyCounted(prey))
            {
                

                if (!prey._isAlive)
                {
                    _hunterData._deadBoids.Add(prey);
                    return;
                }
                _hunterData._posiblePreys.Add(prey);
                

            }
            else
            {

                if (!prey._isAlive)
                {
                    _hunterData._deadBoids.Remove(prey);
                    return;
                }
                _hunterData._posiblePreys.Remove(prey);
            }
        }
    }

    private bool CheckIsIsAlreadyCounted(MS_BoidControlScript boid)
    {
        return _hunterData._deadBoids.Contains(boid) ||  _hunterData._posiblePreys.Contains(boid);
    }

    protected override void ValidTargetOut(Collider target)
    {
        GameObject boid = target.gameObject.transform.parent.gameObject;
        if (boid.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
            if (!prey._isAlive && _hunterData._deadBoids.Contains(prey))
            {
                _hunterData._deadBoids.Remove(prey);
                return;
            }
            else if (_hunterData._posiblePreys.Contains(prey))
            {
                _hunterData._posiblePreys.Remove(prey);
            }
        }
    }
}
