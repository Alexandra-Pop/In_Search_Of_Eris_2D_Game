using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    private GameObject _gameOverScreen;

    private AudioSourcesManager _audioSourcesManager;

    [SerializeField]
    private int _checkpointNumber;

    // Start is called before the first frame update
    private void Start()
    {
        _gameOverScreen = GameObject.FindGameObjectWithTag("gameOverScreen");
        _gameOverScreen.SetActive(false);

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        _gameOverScreen.SetActive(true);

        _audioSourcesManager.StopBackgroundAudio();
        _audioSourcesManager.StopMainCharacterJumpEffect();
        _audioSourcesManager.StopMainCharacterDashEffect();
        _audioSourcesManager.StopStarHitsObjectEffect();
        _audioSourcesManager.StopPuzzleWrongAnswerEffect();
        _audioSourcesManager.StopPuzzleCorrectAnswerEffect();
        _audioSourcesManager.StopStarDissapearsEffect();
        _audioSourcesManager.StopStarCollectingEffect();
        _audioSourcesManager.StopDoorOpeningEffect();
        _audioSourcesManager.StopCheckpointEffect();
        _audioSourcesManager.StartGameOverAudio();
    }

    public void RetryLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void DeleteHiddenStarsFromPrefs()
    {
        int Index = 1;
        bool HasKey = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "HiddenStar" + Index);

        while (HasKey)
        {
            PlayerPrefs.DeleteKey("Checkpoint" + _checkpointNumber.ToString() + "HiddenStar" + Index);

            Index++;
            HasKey = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "HiddenStar" + Index);
        }
    }

    private void DeleteDestructibleObjectsFromPrefs()
    {
        int Index = 1;
        bool HasKey = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "Destructible" + Index);

        while (HasKey)
        {
            PlayerPrefs.DeleteKey("Checkpoint" + _checkpointNumber.ToString() + "Destructible" + Index);

            Index++;
            HasKey = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "Destructible" + Index);
        }
    }

    public void DeleteCheckpoint()
    {
        string CheckpointKeyX = "Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionX";
        string CheckpointKeyY = "Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionY";
        string CheckpointThrows = "Checkpoint" + _checkpointNumber.ToString() + "Throws";
        PlayerPrefs.DeleteKey(CheckpointKeyX);
        PlayerPrefs.DeleteKey(CheckpointKeyY);
        PlayerPrefs.DeleteKey(CheckpointThrows);
        DeleteHiddenStarsFromPrefs();
        DeleteDestructibleObjectsFromPrefs();
    }

    public void ResetLevel()
    {
        DeleteCheckpoint();

        int Value = _checkpointNumber - 1;
        string CheckpointLevelFinished = "Level" + Value.ToString() + "Finished";
        PlayerPrefs.DeleteKey(CheckpointLevelFinished);

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToTitleScreen()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - _checkpointNumber);
    }
}