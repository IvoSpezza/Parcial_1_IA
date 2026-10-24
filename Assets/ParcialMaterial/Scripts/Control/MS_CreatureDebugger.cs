using TMPro;
using UnityEngine;

public class MS_CreatureDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _debugText;
    
    [Header("Offset sobre el agente")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

    private Transform _followThis;

    private void Start()
    {
        if (transform.parent != null)
        {
            _followThis = transform.parent;
             _offset = transform.localPosition;
            transform.SetParent(null);
        }
        else
        {
            Debug.LogWarning("El MS_CreatureDebugger no tiene un agente padre asignado.", this);
        }
    }

    private void LateUpdate()
    {
        if (_followThis == null) return;
        transform.position = _followThis.position + _offset;
    }

    public void SetTitle(string stateName, string name, Color textColor)
    {
        _titleText.text = stateName + " | " + name;
        _titleText.color = textColor;
    }

    public void selectedDebug(string text, Color debugColor)
    {
        _debugText.color = debugColor;
        _debugText.text = text;
    }
}