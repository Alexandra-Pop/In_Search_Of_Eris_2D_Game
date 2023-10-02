using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsLogicScript : MonoBehaviour
{
    private LevelLoadingScript _levelLoadingScript;

    // Start is called before the first frame update
    private void Start()
    {
        _levelLoadingScript = GameObject.FindGameObjectWithTag("levelLoading").GetComponentInParent<LevelLoadingScript>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void StartGame()
    {
        _levelLoadingScript.LoadGivenLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}