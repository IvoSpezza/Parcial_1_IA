using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class VisionGizmos : MonoBehaviour
{
    private FieldOfView _sensor;

    private Vector3 DirFromAngle(float angleDegrees)
    {
        float angleRad = (transform.eulerAngles.y + angleDegrees) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
    }

    private void OnDrawGizmosSelected()
    {
        if (_sensor == null) _sensor = GetComponent<FieldOfView>();
        if (_sensor == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sensor.ViewRadius);

        Vector3 left = DirFromAngle(-_sensor.ViewAngle * 0.5f);
        Vector3 right = DirFromAngle(_sensor.ViewAngle * 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * _sensor.ViewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * _sensor.ViewRadius);

        /*
        if(_sensor.target != null)
        {
            Vector3 eyepos = transform.position + Vector3.up * _sensor.EyeHeight;
            Vector3 endOfSigth = _sensor.target.position;
            endOfSigth.y = eyepos.y;            
            Gizmos.color = _sensor._canSeeTarget ? Color.green : Color.gray;
            Gizmos.DrawLine(eyepos * _sensor.EyeHeight, endOfSigth);
        }
        */
    }
}
