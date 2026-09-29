using UnityEngine;

public class AtackDataGizmo : MonoBehaviour
{
    [Header("Attack Data")]
    [SerializeField] private AtackDatta _atackData;

    [Header("Gizmo Settings")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private bool _showLabels = true;

    [Header("Colors")]
    [SerializeField] private Color _meleeAttackColor = Color.red;
    [SerializeField] private Color _meleePursuitColor = Color.yellow;
    [SerializeField] private Color _rangedColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (!_showGizmos || _atackData == null)
            return;

        Vector3 center = transform.position;

        // Rango de ataque melee
        DrawCircle(
            center,
            _atackData._mad,
            _meleeAttackColor
        );

        // Rango de persecución melee
        DrawCircle(
            center,
            _atackData._mPr,
            _meleePursuitColor
        );

        // Rango de ataque a distancia
        DrawCircle(
            center,
            _atackData._rad,
            _rangedColor
        );

#if UNITY_EDITOR
        if (_showLabels)
        {
            UnityEditor.Handles.color = _meleeAttackColor;
            UnityEditor.Handles.Label(
                center + Vector3.forward * _atackData._mad,
                $"Melee Attack: {_atackData._mad:F1}"
            );

            UnityEditor.Handles.color = _meleePursuitColor;
            UnityEditor.Handles.Label(
                center + Vector3.forward * _atackData._mPr,
                $"Melee Pursuit: {_atackData._mPr:F1}"
            );

            UnityEditor.Handles.color = _rangedColor;
            UnityEditor.Handles.Label(
                center + Vector3.forward * _atackData._rad,
                $"Ranged: {_atackData._rad:F1}"
            );
        }
#endif
    }

    private void DrawCircle(Vector3 center, float radius, Color color)
    {
        Gizmos.color = color;

        Gizmos.DrawWireSphere(
            center,
            radius
        );
    }
}