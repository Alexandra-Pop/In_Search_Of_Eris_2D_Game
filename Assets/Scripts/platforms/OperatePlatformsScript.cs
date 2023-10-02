using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatePlatformsScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _platforms;

    private List<IEnumerator> _coroutines;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (GameObject Platform in _platforms)
        {
            Platform.AddComponent<VerticalMovement>();
            Platform.AddComponent<ReturnMovement>();
        }
        _coroutines = new List<IEnumerator>();
    }

    public void OperatePlatformsOn()
    {
        StopCoroutines();
        foreach (GameObject Platform in _platforms)
        {
            PlatformScript PlatformScript = Platform.GetComponent<PlatformScript>();
            IEnumerator Coroutine = PlatformScript.ApplyStrategy(Platform.GetComponent<VerticalMovement>());
            _coroutines.Add(Coroutine);
            StartCoroutine(Coroutine);
        }
    }

    public void OperatePlatformsOff()
    {
        StopCoroutines();
        foreach (GameObject Platform in _platforms)
        {
            PlatformScript PlatformScript = Platform.GetComponent<PlatformScript>();
            IEnumerator Coroutine = PlatformScript.ApplyStrategy(Platform.GetComponent<ReturnMovement>());
            _coroutines.Add(Coroutine);
            StartCoroutine(Coroutine);
        }
    }

    private void StopCoroutines()
    {
        foreach (IEnumerator Coroutine in _coroutines)
        {
            StopCoroutine(Coroutine);
        }
        _coroutines.Clear();
    }
}