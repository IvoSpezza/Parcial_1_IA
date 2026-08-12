using UnityEngine;

public class Seek_Flee : MonoBehaviour
{
    //Temp
    [SerializeField] private Transform _objetive;

    [SerializeField] private float _maxSpeed = 5;
    [SerializeField] private float _steeringSpeed = 3;

    [SerializeField] private bool _fleeOrSeek = false; 

    private Vector3 actualVelocity;

    void Update()
    {
        if (_fleeOrSeek)
        {
            Flee();
        } 
        else
        {
            Seek();
        }

            transform.position += actualVelocity * Time.deltaTime;
        transform.forward = actualVelocity;
    }

    public void Seek()
    {
        Vector3 desired = (_objetive.position - transform.position).normalized;

        desired *= _maxSpeed;

        Vector3 Steering = desired - actualVelocity;

        Steering = Vector3.ClampMagnitude(Steering, _steeringSpeed * Time.deltaTime);

        actualVelocity += Steering;
    }

    public void Flee()
    {
        Vector3 desired = (_objetive.position - transform.position).normalized;

        desired *= _maxSpeed * -1;

        Vector3 Steering = desired - actualVelocity;

        Steering = Vector3.ClampMagnitude(Steering, _steeringSpeed * Time.deltaTime);

        actualVelocity += Steering;
    }
}
