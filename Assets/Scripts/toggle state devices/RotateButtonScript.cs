using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateButtonScript : MonoBehaviour
{
    private GameObject _platform;
    private PlatformScript _platformScript;
    private List<IMovement> _movements;

    private bool _isPressed;

    [SerializeField]
    private Sprite NotPressedButtonImage;

    [SerializeField]
    private Sprite PressedButtonImage;

    private SpriteRenderer _buttonSprite;
    private PolygonCollider2D _objectCollider;

    private bool _enteredPlayer;
    private bool _enteredBox;

    private AudioSourcesManager _audioSourcesManager;

    private GameObject _timerCanvas;
    private Slider _timerSlider;

    // Start is called before the first frame update
    private void Start()
    {
        _platform = transform.parent.transform.GetChild(0).gameObject;
        _platformScript = _platform.GetComponent<PlatformScript>();
        _movements = new List<IMovement>();
        _movements.Add(_platform.AddComponent<RotateMovement>());

        _buttonSprite = GetComponent<SpriteRenderer>();
        _objectCollider = GetComponent<PolygonCollider2D>();

        _enteredPlayer = false;
        _enteredBox = false;

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();

        _timerCanvas = transform.parent.transform.GetChild(2).gameObject;
        _timerCanvas.SetActive(false);
        _timerSlider = _timerCanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Slider>();
        _timerSlider.maxValue = _platformScript.RotationTime;
    }

    private void ModifyColliderShape()
    {
        _objectCollider.pathCount = _buttonSprite.sprite.GetPhysicsShapeCount();
        List<Vector2> path = new List<Vector2>();

        for (int i = 0; i < _objectCollider.pathCount; i++)
        {
            path.Clear();
            _buttonSprite.sprite.GetPhysicsShape(i, path);
            _objectCollider.SetPath(i, path.ToArray());
        }
    }

    private IEnumerator WaitForPlayerToExit()
    {
        yield return new WaitUntil(() => _enteredPlayer == false);
        _enteredBox = true;
        _isPressed = true;
        _buttonSprite.sprite = PressedButtonImage;
        ModifyColliderShape();

        StartCoroutine(StartTimer());
        StartCoroutine(_platformScript.ApplyStrategy(_movements[0]));
        _audioSourcesManager.StartButtonClickDownEffect();
        yield return new WaitForSeconds(_platformScript.RotationTime);

        _enteredBox = false;
        _isPressed = false;
        _buttonSprite.sprite = NotPressedButtonImage;
        ModifyColliderShape();

        _audioSourcesManager.StartButtonClickUpEffect();
    }

    private IEnumerator WaitForBoxToExit()
    {
        yield return new WaitUntil(() => _enteredBox == false);
        _enteredPlayer = true;
        _isPressed = true;
        _buttonSprite.sprite = PressedButtonImage;
        ModifyColliderShape();

        StartCoroutine(StartTimer());
        StartCoroutine(_platformScript.ApplyStrategy(_movements[0]));
        _audioSourcesManager.StartButtonClickDownEffect();
        yield return new WaitForSeconds(_platformScript.RotationTime);

        _enteredBox = false;
        _isPressed = false;
        _buttonSprite.sprite = NotPressedButtonImage;
        ModifyColliderShape();

        _audioSourcesManager.StartButtonClickUpEffect();
    }

    private IEnumerator StartTimer()
    {
        float CurrentTime = Time.time;
        float Diff = Mathf.Abs(CurrentTime - Time.time);
        _timerCanvas.SetActive(true);

        while (Diff < _platformScript.RotationTime)
        {
            _timerSlider.value = _timerSlider.maxValue - Diff;
            Diff = Mathf.Abs(CurrentTime - Time.time);
            yield return null;
        }
        _timerCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player or the grabbable object interacts with the collider, change the sprite and set
        // the IsPressed flag to true:
        if (collision.gameObject.layer == 8 && !_isPressed)
        {
            StartCoroutine(WaitForBoxToExit());
        }
        if (collision.gameObject.layer == 6 && !_isPressed)
        {
            StartCoroutine(WaitForPlayerToExit());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _buttonSprite.sprite = PressedButtonImage;
        ModifyColliderShape();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_isPressed == false)
        {
            _buttonSprite.sprite = NotPressedButtonImage;
            ModifyColliderShape();
        }
    }
}