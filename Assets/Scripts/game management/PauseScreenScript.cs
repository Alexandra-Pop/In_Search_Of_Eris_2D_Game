using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScreenScript : MonoBehaviour
{
    private GameObject _pauseScreen;

    private AudioSourcesManager _audioSourcesManager;

    [SerializeField]
    private int _checkpointNumber;

    // Start is called before the first frame update
    private void Start()
    {
        _pauseScreen = GameObject.FindGameObjectWithTag("pauseScreen");
        _pauseScreen.SetActive(false);

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Escape has an effect only if the pause screen page is active (so it does't interfere with other functionalities that stop the time,
        // like the dialogues with the npcs):
        if (Input.GetKeyDown(KeyCode.Escape) && _pauseScreen.activeSelf)
        {
            Time.timeScale = 1;
            _pauseScreen.SetActive(false);

            _audioSourcesManager.StopPauseScreenAudio();
            _audioSourcesManager.StartBackgroundAudio();
        }

        // Same thing for the P button to activate the pause screen:
        if (Input.GetKeyDown(KeyCode.P) && !_pauseScreen.activeSelf)
        {
            Time.timeScale = 0;
            _pauseScreen.SetActive(true);

            _audioSourcesManager.StopBackgroundAudio();
            _audioSourcesManager.StopMainCharacterJumpEffect();
            _audioSourcesManager.StopMainCharacterDashEffect();
            _audioSourcesManager.StopStarHitsObjectEffect();
            _audioSourcesManager.StopPuzzleWrongAnswerEffect();
            _audioSourcesManager.StopPuzzleCorrectAnswerEffect();
            _audioSourcesManager.StopStarDissapearsEffect();
            _audioSourcesManager.StopStarCollectingEffect();
            _audioSourcesManager.StopDoorOpeningEffect();
            _audioSourcesManager.StartPauseScreenAudio();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        _pauseScreen.SetActive(false);

        _audioSourcesManager.StopPauseScreenAudio();
        _audioSourcesManager.StartBackgroundAudio();
    }

    public void ReturnToSelectLevelsScreen()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - _checkpointNumber);
    }
}