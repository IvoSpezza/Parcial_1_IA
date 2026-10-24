using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HunterView : FieldOfView 
{
    private HunterData _hunterData;
    private void Start()
    {
        _hunterData = GetComponent<MS_Hunter>()._hunterData;
        _hunterData._entitySeen = new();
        _hunterData._actualPoints = 0;
        _hunterData._maxPointsReached = false;
    }
    protected override void ValidTargetIn(Collider target)
    {
        if(!CanSeeTarget(target.transform)) return;
                

        if(target.transform.parent.gameObject.TryGetComponent<MS_Entity>(out MS_Entity entity))
        {
            _hunterData._entitySeen.Add(entity);
            return;
        }

        if (target.transform.parent.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
            if (!prey._isAlive)
            {
                _hunterData._deadBoids.Add(prey);
                return;
            }           
            _hunterData._posiblePreys.Add(prey);          
        }
    }

    private void OnTriggerStay(Collider target)
    {
        if(!GlobalFunc.IsValidTarget(target.gameObject.layer,_targetMask)) return;
       
        if (CanSeeTarget(target.transform))
        {
            if (target.transform.parent.gameObject.TryGetComponent<MS_Entity>(out MS_Entity entity))
            {
                _hunterData._entitySeen.Add(entity);
                return;
            }

            if (target.transform.parent.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
            {
                if (!CheckIsIsAlreadyCounted(prey))
                {
                    if (!prey._isAlive)
                    {
                        _hunterData._deadBoids.Add(prey);
                        return;
                    }
                    _hunterData._posiblePreys.Add(prey);
                }                
            }

        }
        else
        {
            if (target.transform.parent.gameObject.TryGetComponent<MS_Entity>(out MS_Entity entity))
            {
                _hunterData._entitySeen.Remove(entity);
                return;
            }

            if (target.transform.parent.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey)) 
            {
                if(CheckIsIsAlreadyCounted(prey))
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

    }

    private bool CheckIsIsAlreadyCounted(MS_BoidControlScript boid)
    {
        return _hunterData._deadBoids.Contains(boid) ||  _hunterData._posiblePreys.Contains(boid);
    }

    protected override void ValidTargetOut(Collider target)
    {
        

        if (target.transform.parent.gameObject.TryGetComponent<MS_Entity>(out MS_Entity entity))
        {
            _hunterData._entitySeen.Remove(entity);
            return;
        }

        if (target.transform.parent.gameObject.TryGetComponent<MS_BoidControlScript>(out MS_BoidControlScript prey))
        {
            if (!prey._isAlive && _hunterData._deadBoids.Contains(prey))
            {
                _hunterData._deadBoids.Remove(prey);
                return;
            }
            else if (_hunterData._posiblePreys.Contains(prey))
            {
                _hunterData._posiblePreys.Remove(prey);
                return;
            }
        }
    }
}
