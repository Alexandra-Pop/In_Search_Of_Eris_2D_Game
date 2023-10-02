using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockedDoorScript : MonoBehaviour
{
    private bool _canOpen;
    private GameObject _key;
    private GameObject _character;

    [SerializeField]
    private Vector3 _position;

    private GameObject _arrow;
    private GameObject _infoCanvas;

    [SerializeField]
    private Sprite _openedImage;

    private GameObject _lock;

    private AudioSourcesManager _audioSourcesManager;
    private LevelTransitionsScript _levelTransitionsScript;
    private GameOverScript _gameOverScript;

    // Start is called before the first frame update
    private void Start()
    {
        _canOpen = false;

        _arrow = transform.GetChild(0).gameObject;
        _arrow.SetActive(false);
        _infoCanvas = transform.GetChild(1).gameObject;
        _infoCanvas.SetActive(false);

        _lock = transform.GetChild(2).gameObject;
        _lock.SetActive(false);

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();
        _levelTransitionsScript = GameObject.FindGameObjectWithTag("levelTransitions").GetComponent<LevelTransitionsScript>();
        _gameOverScript = GameObject.FindGameObjectWithTag("gameOverScript").GetComponent<GameOverScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        if (_canOpen && Input.GetKeyDown(KeyCode.Return))
        {
            BoxCollider2D _collider = gameObject.GetComponent<BoxCollider2D>();
            _collider.enabled = false;
            _audioSourcesManager.StartDoorOpeningEffect();
            yield return new WaitForSeconds(1.5f);
            gameObject.GetComponent<SpriteRenderer>().sprite = _openedImage;
            _lock.SetActive(true);
            Destroy(_key);
            Destroy(_arrow);
            Destroy(_infoCanvas);
            yield return new WaitForSeconds(1f);
            // delete first checkpoint:
            _gameOverScript.DeleteCheckpoint();
            // transition to new position:
            _levelTransitionsScript.StartSameLevelTransition();
            yield return new WaitForSeconds(_levelTransitionsScript.TransitionTime);
            // move the character game object to the new position (part 2 of the level):
            _character.transform.position = new Vector3(343, 8, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 8) && (collision.gameObject.transform.childCount == 2))
        {
            _canOpen = true;
            _key = collision.gameObject.transform.GetChild(1).gameObject;
            _character = collision.gameObject;

            _arrow.SetActive(true);
            _infoCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _canOpen = false;

            _arrow.SetActive(false);
            _infoCanvas.SetActive(false);
        }
    }
}