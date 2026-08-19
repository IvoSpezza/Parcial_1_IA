using UnityEngine;
using UnityEngine.InputSystem;

public class WASDmovement : Agent
{
    [SerializeField] private float _maxSpeed = 5;

    void Update()
    {
        transform.position += _actualVelocity * Time.deltaTime;
    }

    public void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        _actualVelocity = new Vector3(movement.x, 0, movement.y) * _maxSpeed;
    }
}
