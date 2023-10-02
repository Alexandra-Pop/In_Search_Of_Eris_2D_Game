using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    private GameObject _arrow;
    private GameObject _infoCanvas;

    [SerializeField]
    private Sprite _offImage;

    [SerializeField]
    private Sprite _onImage;

    private bool _collided;

    private bool _state;

    private OperatePlatformsScript _operatePlatformsScript;

    // Start is called before the first frame update
    private void Start()
    {
        _arrow = transform.GetChild(0).gameObject;
        _arrow.SetActive(false);
        _infoCanvas = transform.GetChild(1).gameObject;
        _infoCanvas.SetActive(false);

        gameObject.GetComponent<SpriteRenderer>().sprite = _offImage;

        _collided = false;
        _state = false;

        _operatePlatformsScript = gameObject.transform.parent.transform.GetChild(0).GetComponent<OperatePlatformsScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        UseSwitch();
    }

    private void UseSwitch()
    {
        if (_collided && Input.GetKeyDown(KeyCode.Return))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = _state == false ? _onImage : _offImage;
            _state = !_state;
            if (_state)
            {
                _operatePlatformsScript.OperatePlatformsOn();
            }
            else
            {
                _operatePlatformsScript.OperatePlatformsOff();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _arrow.SetActive(true);
        _infoCanvas.SetActive(true);

        _collided = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _arrow.SetActive(false);
        _infoCanvas.SetActive(false);

        _collided = false;
    }
}