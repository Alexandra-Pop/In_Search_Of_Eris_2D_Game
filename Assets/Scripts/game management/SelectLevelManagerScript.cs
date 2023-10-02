using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SelectLevelManagerScript : MonoBehaviour
{
    private LevelLoadingScript _levelLoadingScript;

    [SerializeField]
    private Image _tutorialFinishedIcon;

    [SerializeField]
    private Image _level1FinishedIcon;

    [SerializeField]
    private Image _level1Image;

    [SerializeField]
    private Button _level1Button;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(DisableAnimator());

        _levelLoadingScript = GameObject.FindGameObjectWithTag("levelLoading").GetComponentInParent<LevelLoadingScript>();

        CheckLevel1Availability();

        CheckIfTutorialIsFinished();
        CheckLevel1IsFinished();
    }

    private IEnumerator DisableAnimator()
    {
        // The transition animation sits on top of the main canvas, so when the animation is finished, we set the object as inactive so
        // it doesn't interfere with the main canvas' buttons (we wait a number of seconds equal to the animation transition time):
        GameObject _transitionObject = GameObject.FindGameObjectWithTag("levelTransitions");
        float TransitionTime = _transitionObject.GetComponent<LevelTransitionsScript>().TransitionTime;
        yield return new WaitForSeconds(TransitionTime);
        _transitionObject.SetActive(false);
    }

    private void CheckLevel1Availability()
    {
        // if the tutorial isn't finished, dau disable la buton si dau o culoare gri imaginii:
        bool HasFinishedTutorialKey = PlayerPrefs.HasKey("Level0Finished");
        _level1Image.color = HasFinishedTutorialKey ? new Color(_level1Image.color.r, _level1Image.color.g, _level1Image.color.b, 255)
            : Color.gray;
        _level1Button.enabled = HasFinishedTutorialKey ? true : false;
    }

    public void GoToTitleScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ResetGame()
    {
        // If we want to restart the game, it means we have to restart all the levels, so we delete all the user's preferences:
        PlayerPrefs.DeleteAll();
        CheckIfTutorialIsFinished();
        CheckLevel1IsFinished();
        CheckLevel1Availability();

        // Deselect the "reset game" button so the "normal" animation continues to play:
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PlayTutorial()
    {
        // If we want to play the tutorial, we'll load the scene async with the help of the Level Loading Prefab:
        _levelLoadingScript.LoadGivenLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayLevel1()
    {
        // If the tutorial is finished, if we press on the image, play level 1:
        _levelLoadingScript.LoadGivenLevel(SceneManager.GetActiveScene().buildIndex + 2);
    }

    private void CheckIfTutorialIsFinished()
    {
        _tutorialFinishedIcon.enabled = PlayerPrefs.HasKey("Level0Finished") ? true : false;
    }

    private void CheckLevel1IsFinished()
    {
        _level1FinishedIcon.enabled = PlayerPrefs.HasKey("Level1Finished") ? true : false;
    }
}