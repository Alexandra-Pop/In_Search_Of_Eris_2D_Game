using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutsceneLevel1 : MonoBehaviour
{
    private MainCharacterScript _mainCharacterScript;

    private GameObject _platform;
    private PlatformScript _platformScript;

    private CinemachineVirtualCamera _gameCamera;

    [SerializeField]
    private int _checkpointNumber;

    // Start is called before the first frame update
    private void Start()
    {
        _mainCharacterScript = GameObject.FindGameObjectWithTag("throwableObjectPosition").GetComponentInParent<MainCharacterScript>();
        _platform = transform.GetChild(0).gameObject;
        _platform.AddComponent<VerticalMovement>();
        _platform.AddComponent<InstantMovement>();
        _platformScript = _platform.GetComponent<PlatformScript>();

        _gameCamera = GameObject.FindGameObjectWithTag("followCamera").GetComponent<CinemachineVirtualCamera>();

        CheckForCheckpoint();
    }

    private void CheckForCheckpoint()
    {
        string CheckpointKeyX = "Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionX";
        string CheckpointKeyY = "Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionY";
        bool KeyOnX = PlayerPrefs.HasKey(CheckpointKeyX);
        bool KeyOnY = PlayerPrefs.HasKey(CheckpointKeyY);
        if (!KeyOnX && !KeyOnY)
        {
            StartCoroutine(StartCutscene());
        }
        else
        {
            _platformScript.ApplyStrategy(_platform.GetComponent<InstantMovement>());
        }
    }

    private IEnumerator StartCutscene()
    {
        _mainCharacterScript.gameObject.transform.SetParent(_platform.transform);
        yield return new WaitUntil(() => _mainCharacterScript.CanMove == true);
        _mainCharacterScript.CanMove = false;

        _gameCamera.m_Lens.OrthographicSize = 9f;

        yield return StartCoroutine(_platformScript.ApplyStrategy(_platform.GetComponent<VerticalMovement>()));

        _gameCamera.m_Lens.OrthographicSize = 12.1f;

        _mainCharacterScript.gameObject.transform.SetParent(null);
        _mainCharacterScript.CanMove = true;
    }
}