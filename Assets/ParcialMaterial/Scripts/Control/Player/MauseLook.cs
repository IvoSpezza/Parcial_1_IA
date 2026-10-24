using UnityEngine;
using UnityEngine.InputSystem;

public class MauseLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float minPitch = -89f;
    [SerializeField] private float maxPitch = 89f;
    [SerializeField] private Transform CamPos;

    private Transform _camera;
    private float _pitch;

    void Awake()
    {
        _camera = Camera.main.transform;        
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Delta del mouse con el nuevo Input System
        Vector2 delta = Mouse.current.delta.ReadValue();

        // Yaw rota al player (eje Y)
        transform.Rotate(Vector3.up * delta.x * sensitivity * Time.deltaTime);

        // Pitch rota a la cámara (eje X local)
        _pitch -= delta.y * sensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        _camera.localRotation = Quaternion.Euler(_pitch, delta.y, 0f);
    }

    private void LateUpdate()
    {
        _camera.position = CamPos.position;
        _camera.rotation = CamPos.rotation;
    }
}
