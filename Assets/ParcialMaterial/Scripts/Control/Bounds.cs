using UnityEngine;

public class Bounds : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private float _width;
    [SerializeField] private bool _drawGizmos;
    public static Bounds instance { get; private set; }

    

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public Vector3 OutOfBounds(Vector3 position)
    {
        Vector3 newPosition = position;

        if (position.x > _width / 2) newPosition.x = -_width / 2;
        if (position.x < -_width / 2) newPosition.x = _width / 2;
        if (position.z > _height / 2) newPosition.z = -_height / 2;
        if (position.z < -_height / 2) newPosition.z = _height / 2;

        return newPosition;
    }

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(_width,0,_height));
        }
    }
}
