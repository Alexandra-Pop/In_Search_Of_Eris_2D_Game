using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public bool IsPressed;

    [SerializeField]
    private Sprite NotPressedButtonImage;

    [SerializeField]
    private Sprite PressedButtonImage;

    private SpriteRenderer _buttonSprite;
    private PolygonCollider2D _objectCollider;

    private DoorScript _doorScript;

    private bool _enteredPlayer;
    private bool _enteredBox;

    private AudioSourcesManager _audioSourcesManager;

    // Start is called before the first frame update
    private void Start()
    {
        IsPressed = false;
        _buttonSprite = GetComponent<SpriteRenderer>();
        _objectCollider = GetComponent<PolygonCollider2D>();

        _doorScript = GameObject.FindGameObjectWithTag("door").GetComponentInParent<DoorScript>();

        _enteredPlayer = false;
        _enteredBox = false;

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();
    }

    // Update is called once per frame
    private void Update()
    {
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
        _buttonSprite.sprite = PressedButtonImage;
        ModifyColliderShape();
        _doorScript.AddToPressedButtonsOrder(gameObject);
        _audioSourcesManager.StartButtonClickDownEffect();
    }

    private IEnumerator WaitForBoxToExit()
    {
        yield return new WaitUntil(() => _enteredBox == false);
        _enteredPlayer = true;
        _buttonSprite.sprite = PressedButtonImage;
        ModifyColliderShape();
        _doorScript.AddToPressedButtonsOrder(gameObject);
        _audioSourcesManager.StartButtonClickDownEffect();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player or the grabbable object interacts with the collider, change the sprite and set
        // the IsPressed flag to true:
        if (collision.gameObject.layer == 8)
        {
            StartCoroutine(WaitForBoxToExit());
        }
        if (collision.gameObject.layer == 6)
        {
            StartCoroutine(WaitForPlayerToExit());
        }
    }

    private IEnumerator WaitForBoxToEnter()
    {
        yield return new WaitUntil(() => _enteredBox == true);
        IsPressed = false;
        _enteredBox = false;
        _buttonSprite.sprite = NotPressedButtonImage;
        ModifyColliderShape();
        _doorScript.DeleteFromPressedButtonsOrder(gameObject);
        _audioSourcesManager.StartButtonClickUpEffect();
    }

    private IEnumerator WaitForPlayerToEnter()
    {
        yield return new WaitUntil(() => _enteredPlayer == true);
        IsPressed = false;
        _enteredPlayer = false;
        _buttonSprite.sprite = NotPressedButtonImage;
        ModifyColliderShape();
        _doorScript.DeleteFromPressedButtonsOrder(gameObject);
        _audioSourcesManager.StartButtonClickUpEffect();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            StartCoroutine(WaitForPlayerToEnter());
        }
        if (collision.gameObject.layer == 6)
        {
            StartCoroutine(WaitForBoxToEnter());
        }
    }
}