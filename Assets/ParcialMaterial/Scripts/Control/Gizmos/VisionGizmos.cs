using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class VisionGizmos : MonoBehaviour
{
    private FieldOfView _sensor;
    private MS_Creature _creature;

    private Vector3 DirFromAngle(float angleDegrees)
    {
        float angleRad = (transform.eulerAngles.y + angleDegrees) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
    }

    private void OnDrawGizmosSelected()
    {
        if (_sensor == null) _sensor = GetComponent<FieldOfView>();
        if (_sensor == null) return;

        if(_creature == null) _creature = GetComponent<MS_Creature>();
        if (_creature == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sensor.ViewRadius);

        Vector3 left = DirFromAngle(-_sensor.ViewAngle * 0.5f);
        Vector3 right = DirFromAngle(_sensor.ViewAngle * 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * _sensor.ViewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * _sensor.ViewRadius);

        left = DirFromAngle(-_creature.AvoidAngle * 0.5f);
        right = DirFromAngle(_creature.AvoidAngle * 0.5f);
        Vector3 pos = transform.position + Vector3.up * _sensor.EyeHeight;
        Gizmos.color = Color.pink;
        Gizmos.DrawLine(pos, pos + left * _creature.AvoidDistance);
        Gizmos.DrawLine(pos, pos + right * _creature.AvoidDistance);
        ;
    }
}
