using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;


    private Rigidbody _rigidbody;
    private bool _rigid = true;

    private Rigidbody _rigidBody;    

    private Vector3 _movementInput;
    private Vector3 _movement
    {
        get
        {
            return (transform.forward * _movementInput.z + transform.right * _movementInput.x) * speed;
        }
        set
        {
            _movementInput = value;
        }        
    }
    public Vector3 _velocity => _movement;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_rigid)
        {
            _rigidbody.linearVelocity += _movement * Time.deltaTime;
        }
        else
        {
            transform.position += _movementInput * speed * Time.deltaTime;
        }
    }
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        _movement = new Vector3(input.x,0,input.y);
    }

    public void OnSprint()
    {
        _rigid = !_rigid;
    }
}
