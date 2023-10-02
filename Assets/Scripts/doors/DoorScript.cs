using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;

public class DoorScript : MonoBehaviour
{
    private SpriteRenderer _buttonChild1; // blue button;
    private SpriteRenderer _buttonChild2; // green button;
    private SpriteRenderer _buttonChild3; // purple button;

    private int[] _buttonsOrder;
    private List<int> _pressedButtonsOrder;

    private float _instantiatedTime;

    private float _lastShowHintTime;

    private Animator _doorAnimation;

    private bool _openedDoor;

    private GameObject _doorArrow;
    private GameObject _doorText;

    private AudioSourcesManager _audioSourcesManager;

    private CinemachineVirtualCamera _gameCamera;

    private bool _canOpen;

    private LevelTransitionsScript _levelTransitionsScript;

    private GameObject _starsCanvas;

    [SerializeField]
    private int _checkpointNumber;

    [SerializeField]
    private float _cameraDistance;

    [SerializeField]
    private float _initialCameraDistance;

    // Start is called before the first frame update
    private void Start()
    {
        _buttonChild1 = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        _buttonChild1.gameObject.SetActive(false);

        _buttonChild2 = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        _buttonChild2.gameObject.SetActive(false);

        _buttonChild3 = transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
        _buttonChild3.gameObject.SetActive(false);

        _buttonsOrder = new int[3];
        _pressedButtonsOrder = new List<int>();
        GetRandomButtonsOrder();

        _instantiatedTime = Time.time;

        _lastShowHintTime = _instantiatedTime;

        _doorAnimation = GetComponent<Animator>();
        _doorAnimation.enabled = false;

        StartCoroutine(ShowButtons());

        _openedDoor = false;

        _doorArrow = transform.GetChild(3).gameObject;
        _doorArrow.SetActive(false);

        _doorText = transform.GetChild(4).gameObject;
        _doorText.SetActive(false);

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();

        _gameCamera = GameObject.FindGameObjectWithTag("followCamera").GetComponent<CinemachineVirtualCamera>();
        _gameCamera.m_Lens.OrthographicSize = 12.1f;

        _canOpen = false;

        _levelTransitionsScript = GameObject.FindGameObjectWithTag("levelTransitions").GetComponent<LevelTransitionsScript>();

        _starsCanvas = GameObject.FindGameObjectWithTag("starsCanvas");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Mathf.Abs(_instantiatedTime - Time.time) > 120f && !_openedDoor)
        {
            _instantiatedTime = Time.time;
            GetRandomButtonsOrder();
        }

        if (Mathf.Abs(Time.time - _lastShowHintTime) > 2f && !_openedDoor)
        {
            StartCoroutine(ShowButtons());
        }

        if (_pressedButtonsOrder.Count() == 3 && !_openedDoor)
        {
            CanOpenTheDoor();
        }

        FinishLevel();
    }

    private void GetRandomButtonsOrder()
    {
        int Button = Random.Range(0, 3);
        _buttonsOrder[0] = Button;
        do
        {
            Button = Random.Range(0, 3);
        }
        while (Button == _buttonsOrder[0]);
        _buttonsOrder[1] = Button;
        if ((_buttonsOrder[0] == 0 || _buttonsOrder[0] == 1) && (_buttonsOrder[1] == 0 || _buttonsOrder[1] == 1))
        {
            _buttonsOrder[2] = 2;
        }
        else if ((_buttonsOrder[0] == 1 || _buttonsOrder[0] == 2) && (_buttonsOrder[1] == 1 || _buttonsOrder[1] == 2))
        {
            _buttonsOrder[2] = 0;
        }
        else if ((_buttonsOrder[0] == 0 || _buttonsOrder[0] == 2) && (_buttonsOrder[1] == 0 || _buttonsOrder[1] == 2))
        {
            _buttonsOrder[2] = 1;
        }
    }

    private void WhichButton(int Value)
    {
        switch (Value)
        {
            case 0:
                _buttonChild2.gameObject.SetActive(false);
                _buttonChild3.gameObject.SetActive(false);
                _buttonChild1.gameObject.SetActive(true);
                break;

            case 1:
                _buttonChild1.gameObject.SetActive(false);
                _buttonChild3.gameObject.SetActive(false);
                _buttonChild2.gameObject.SetActive(true);
                break;

            case 2:
                _buttonChild1.gameObject.SetActive(false);
                _buttonChild2.gameObject.SetActive(false);
                _buttonChild3.gameObject.SetActive(true);
                break;
        }
    }

    private IEnumerator ShowButtons()
    {
        foreach (int Value in _buttonsOrder)
        {
            _lastShowHintTime = Time.time;
            WhichButton(Value);
            yield return new WaitForSeconds(0.5f);
        }

        _buttonChild2.gameObject.SetActive(false);
        _buttonChild3.gameObject.SetActive(false);
        _buttonChild1.gameObject.SetActive(false);
    }

    public void AddToPressedButtonsOrder(GameObject Button)
    {
        if (Button.tag == "blueButton")
        {
            _pressedButtonsOrder.Add(0);
        }
        else if (Button.tag == "greenButton")
        {
            _pressedButtonsOrder.Add(1);
        }
        else if (Button.tag == "purpleButton")
        {
            _pressedButtonsOrder.Add(2);
        }
    }

    private void ResetPressedButtonsList()
    {
        _pressedButtonsOrder.Clear();
    }

    public void DeleteFromPressedButtonsOrder(GameObject Button)
    {
        if (Button.tag == "blueButton")
        {
            _pressedButtonsOrder.Remove(0);
        }
        else if (Button.tag == "greenButton")
        {
            _pressedButtonsOrder.Remove(1);
        }
        else if (Button.tag == "purpleButton")
        {
            _pressedButtonsOrder.Remove(2);
        }
    }

    // Zoom out to see the door opening, open the door and then zoom in to the initial lens size:
    private IEnumerator OpeningTheDoor()
    {
        _openedDoor = true;
        _gameCamera.m_Lens.OrthographicSize = _cameraDistance;
        _audioSourcesManager.StartPuzzleCorrectAnswerEffect();
        yield return new WaitForSeconds(0.6f);
        _audioSourcesManager.StartDoorOpeningEffect();
        _doorAnimation.enabled = true;
        yield return new WaitForSeconds(1.6f);
        _gameCamera.m_Lens.OrthographicSize = _initialCameraDistance;
    }

    private void CanOpenTheDoor()
    {
        if (string.Join(", ", _pressedButtonsOrder.ToArray()) == string.Join(", ", _buttonsOrder))
        {
            StartCoroutine(OpeningTheDoor());
        }
        else
        {
            ResetPressedButtonsList();
            _audioSourcesManager.StartPuzzleWrongAnswerEffect();
        }
    }

    private void FinishLevel()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _canOpen)
        {
            int Value = _checkpointNumber - 1;
            // Save that the level is finished in PlayerPrefs:
            PlayerPrefs.SetInt("Level" + Value + "Finished", 1);

            Value = _checkpointNumber + 1;
            // Save the stars for the next level:
            TMP_Text _starsNumberText = _starsCanvas.transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>();
            int NumberOfThrows = int.Parse(_starsNumberText.text.Substring(2, _starsNumberText.text.Length - 2));
            bool hasPrefs = PlayerPrefs.HasKey("Checkpoint" + Value.ToString() + "Throws");
            if (!hasPrefs)
            {
                PlayerPrefs.SetInt("Checkpoint" + Value.ToString() + "Throws", NumberOfThrows);
            }

            // Hide de stars canvas:
            _starsCanvas.SetActive(false);

            // Go to select levels screen and start the end level animation:
            _levelTransitionsScript.LoadGivenScene(SceneManager.GetActiveScene().buildIndex - _checkpointNumber);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_openedDoor)
        {
            _doorArrow.SetActive(true);
            _doorText.SetActive(true);
            _canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_openedDoor)
        {
            _doorArrow.SetActive(false);
            _doorText.SetActive(false);
            _canOpen = false;
        }
    }
}