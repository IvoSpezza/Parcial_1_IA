using UnityEngine;

public class SimulatorManager : MonoBehaviour
{
    [SerializeField] private MS_Hunter _hunterPrefab;

    [SerializeField] private int _bodyCountToWin = 2;

    [SerializeField] private float _respawnTime = 2f;

    [SerializeField] private GameObject[] _entitis; //NO LO UTILIZO PARA NADA MAS QUE ASEGURARME DE QUE LOS BICHOS SE GENEREN CON EL GRAFO YA GENERADO;

    private void Start()
    {
        Graph.Instance.OnGraphReady += GraphReady;
    }

    private void OnDisable()
    {
        _hunterPrefab.OnDie -= HunterDied;
    }     

    private void GraphReady()
    {        
        Graph.Instance.OnGraphReady -= GraphReady;

        foreach (var ent in _entitis) 
        {
            ent.SetActive(true);
        }

        _hunterPrefab = Instantiate(_hunterPrefab);
        _hunterPrefab.transform.position = transform.position;
        _hunterPrefab.OnDie += HunterDied;
    }

    private void HunterDied()
    {         
        _hunterPrefab.transform.position = transform.position;        
    } 
}
