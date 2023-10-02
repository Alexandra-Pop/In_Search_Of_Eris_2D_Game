using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoadingScript : MonoBehaviour
{
    private GameObject _levelLoadingCanvas;

    private Slider _levelLoadingSlider;

    // Start is called before the first frame update
    private void Start()
    {
        _levelLoadingCanvas = transform.GetChild(0).gameObject;
        _levelLoadingCanvas.SetActive(false);

        _levelLoadingSlider = _levelLoadingCanvas.transform.GetChild(0).GetComponent<Slider>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void LoadGivenLevel(int Index)
    {
        _levelLoadingCanvas.SetActive(true);
        StartCoroutine(LoadGivenLevelAsync(Index));
    }

    private IEnumerator LoadGivenLevelAsync(int Index)
    {
        AsyncOperation AsyncLoadOperation = SceneManager.LoadSceneAsync(Index);

        while (!AsyncLoadOperation.isDone)
        {
            float SliderValue = Mathf.Clamp01(AsyncLoadOperation.progress / 0.9f);
            _levelLoadingSlider.value = SliderValue;
            yield return null;
        }
    }
}