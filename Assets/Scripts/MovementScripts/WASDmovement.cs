using UnityEngine;
using UnityEngine.InputSystem;

public class WASDmovement : MonoBehaviour
{

    private Vector2 _movement;
   

    void Update()
    {
        if (Input.GetButtonDown("Up"))
        {
            Debug.Log("sad");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        
        _movement = context.ReadValue<Vector2>();
    }
}
