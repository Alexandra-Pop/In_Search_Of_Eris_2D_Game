using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcesManager : SingletonScript<AudioSourcesManager>
{
    private AudioSource _backgroundAudio;
    private AudioSource _gameOverAudio;
    private AudioSource _pauseScreenAudio;
    private AudioSource _mainCharacterJumpEffect;
    private AudioSource _mainCharacterDashEffect;
    private AudioSource _starHitsObjectEffect;
    private AudioSource _puzzleWrongAnswerEffect;
    private AudioSource _puzzleCorrectAnswerEffect;
    private AudioSource _starDissapearsEffect;
    private AudioSource _starCollectingEffect;
    private AudioSource _doorOpeningEffect;
    private AudioSource _checkpointEffect;
    private AudioSource _buttonClickDownEffect;
    private AudioSource _buttonClickUpEffect;

    // Start is called before the first frame update
    private void Start()
    {
        _backgroundAudio = transform.GetChild(0).GetComponent<AudioSource>();
        _backgroundAudio.Play(); // we play the background song when the scene is loaded;

        _gameOverAudio = transform.GetChild(1).GetComponent<AudioSource>();
        _pauseScreenAudio = transform.GetChild(2).GetComponent<AudioSource>();
        _mainCharacterJumpEffect = transform.GetChild(3).GetComponent<AudioSource>();
        _mainCharacterDashEffect = transform.GetChild(4).GetComponent<AudioSource>();
        _starHitsObjectEffect = transform.GetChild(5).GetComponent<AudioSource>();
        _puzzleWrongAnswerEffect = transform.GetChild(6).GetComponent<AudioSource>();
        _puzzleCorrectAnswerEffect = transform.GetChild(7).GetComponent<AudioSource>();
        _starDissapearsEffect = transform.GetChild(8).GetComponent<AudioSource>();
        _starCollectingEffect = transform.GetChild(9).GetComponent<AudioSource>();
        _doorOpeningEffect = transform.GetChild(10).GetComponent<AudioSource>();
        _checkpointEffect = transform.GetChild(11).GetComponent<AudioSource>();
        _buttonClickDownEffect = transform.GetChild(12).GetComponent<AudioSource>();
        _buttonClickUpEffect = transform.GetChild(12).GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void StartBackgroundAudio()
    {
        _backgroundAudio.Play();
    }

    public void StopBackgroundAudio()
    {
        _backgroundAudio?.Pause();
    }

    public void StartGameOverAudio()
    {
        _gameOverAudio.Play();
    }

    public void StopGameOverAudio()
    {
        _gameOverAudio?.Stop();
    }

    public void StartPauseScreenAudio()
    {
        _pauseScreenAudio.Play();
    }

    public void StopPauseScreenAudio()
    {
        _pauseScreenAudio?.Stop();
    }

    public void StartMainCharacterJumpEffect()
    {
        _mainCharacterJumpEffect.Play();
    }

    public void StopMainCharacterJumpEffect()
    {
        _mainCharacterJumpEffect?.Stop();
    }

    public void StartMainCharacterDashEffect()
    {
        _mainCharacterDashEffect.Play();
    }

    public void StopMainCharacterDashEffect()
    {
        _mainCharacterJumpEffect?.Stop();
    }

    public void StartStarHitsObjectEffect()
    {
        _starHitsObjectEffect.Play();
    }

    public void StopStarHitsObjectEffect()
    {
        _starHitsObjectEffect?.Stop();
    }

    public void StartPuzzleWrongAnswerEffect()
    {
        _puzzleWrongAnswerEffect.Play();
    }

    public void StopPuzzleWrongAnswerEffect()
    {
        _puzzleWrongAnswerEffect?.Stop();
    }

    public void StartPuzzleCorrectAnswerEffect()
    {
        _puzzleCorrectAnswerEffect.Play();
    }

    public void StopPuzzleCorrectAnswerEffect()
    {
        _puzzleCorrectAnswerEffect?.Stop();
    }

    public void StartStarDissapearsEffect()
    {
        _starDissapearsEffect.Play();
    }

    public void StopStarDissapearsEffect()
    {
        _starDissapearsEffect?.Stop();
    }

    public void StartStarCollectingEffect()
    {
        _starCollectingEffect.Play();
    }

    public void StopStarCollectingEffect()
    {
        _starCollectingEffect?.Stop();
    }

    public void StartDoorOpeningEffect()
    {
        _doorOpeningEffect.Play();
    }

    public void StopDoorOpeningEffect()
    {
        _doorOpeningEffect?.Pause();
    }

    public void StartCheckpointEffect()
    {
        _checkpointEffect.Play();
    }

    public void StopCheckpointEffect()
    {
        _checkpointEffect?.Stop();
    }

    public void StartButtonClickDownEffect()
    {
        _buttonClickDownEffect.Play();
    }

    public void StopButtonClickDownEffect()
    {
        _buttonClickDownEffect?.Stop();
    }

    public void StartButtonClickUpEffect()
    {
        _buttonClickUpEffect.Play();
    }

    public void StopButtonClickUpEffect()
    {
        _buttonClickUpEffect?.Stop();
    }
}