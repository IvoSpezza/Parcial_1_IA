using TMPro;
using UnityEngine;

public class MS_CreatureDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _debugText;


    private float _posInZ;
    private float _Ypos;
    private Transform _followThis;

    private void Awake()
    {
        _followThis = transform.parent;
        transform.SetParent(null);
        _Ypos = transform.position.y;
        _posInZ = transform.position.z;
    }

    private void LateUpdate()
    {
        transform.position = _followThis.position;

        transform.position += new Vector3(0, _Ypos, _posInZ);
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
