using UnityEngine;
using UnityEngine.InputSystem;

public class WASDmovement : MonoBehaviour
{
    private Vector2 _movement;
   

    void Update()
    {
        Debug.Log(_movement);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        
        _movement = context.ReadValue<Vector2>();
    }
}
