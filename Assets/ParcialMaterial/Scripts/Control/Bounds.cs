using UnityEngine;

public class Bounds : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private float _width;
    [SerializeField] private bool _drawGizmos;

    public float Heigth => _height;
    public float Width => _width;
    public static Bounds instance { get; private set; }

    

    

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public Vector3 OutOfBounds(Vector3 position)
    {
        // 1. Convertimos la posición global del agente a la posición local respecto a este objeto
        Vector3 localPosition = transform.InverseTransformPoint(position);

        float halfWidth = _width / 2f;
        float halfHeight = _height / 2f;

        // 2. Comprobamos el eje X (izquierda / derecha) y teletransportamos al lado opuesto
        if (localPosition.x > halfWidth)
        {
            localPosition.x = -halfWidth;
        }
        else if (localPosition.x < -halfWidth)
        {
            localPosition.x = halfWidth;
        }

        // 3. Comprobamos el eje Z (superior / inferior) y teletransportamos al lado opuesto
        if (localPosition.z > halfHeight)
        {
            localPosition.z = -halfHeight;
        }
        else if (localPosition.z < -halfHeight)
        {
            localPosition.z = halfHeight;
        }

        // 4. Devolvemos la posición convertida nuevamente a coordenadas globales del mundo
        return transform.TransformPoint(localPosition);
    }

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            Gizmos.color = Color.yellow;
            // Dibujamos el cubo usando la posición y rotación actual del transform
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_width, 0, _height));
        }
    }
}

