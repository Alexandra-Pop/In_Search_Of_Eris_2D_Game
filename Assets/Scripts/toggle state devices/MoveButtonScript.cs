using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButtonScript : MonoBehaviour
{
    private GameObject _platform;
    private PlatformScript _platformScript;
    private List<IMovement> _movements;

    [SerializeField]
    private Sprite NotPressedButtonImage;

    [SerializeField]
    private Sprite PressedButtonImage;

    private SpriteRenderer _buttonSprite;
    private PolygonCollider2D _objectCollider;

    private bool _enteredPlayer;
    private bool _enteredBox;

    private AudioSourcesManager _audioSourcesManager;

    private List<IEnumerator> _coroutines;

    // Start is called before the first frame update
    private void Start()
    {
        _platform = transform.parent.transform.GetChild(0).gameObject;
        _platformScript = _platform.GetComponent<PlatformScript>();
        _movements = new List<IMovement>();
        _movements.Add(_platform.AddComponent<VerticalMovement>());
        _movements.Add(_platform.AddComponent<ReturnMovement>());

        _buttonSprite = GetComponent<SpriteRenderer>();
        _objectCollider = GetComponent<PolygonCollider2D>();

        _enteredPlayer = false;
        _enteredBox = false;

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();

        _coroutines = new List<IEnumerator>();
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

        StopCoroutines();
        IEnumerator Coroutine = _platformScript.ApplyStrategy(_movements[0]);
        _coroutines.Add(Coroutine);
        StartCoroutine(Coroutine);

        _audioSourcesManager.StartButtonClickDownEffect();
    }

    private IEnumerator WaitForBoxToExit()
    {
        yield return new WaitUntil(() => _enteredBox == false);
        _enteredPlayer = true;
        _buttonSprite.sprite = PressedButtonImage;
        ModifyColliderShape();

        StopCoroutines();
        IEnumerator Coroutine = _platformScript.ApplyStrategy(_movements[0]);
        _coroutines.Add(Coroutine);
        StartCoroutine(Coroutine);

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
        _enteredBox = false;
        _buttonSprite.sprite = NotPressedButtonImage;
        ModifyColliderShape();

        StopCoroutines();
        IEnumerator Coroutine = _platformScript.ApplyStrategy(_movements[1]);
        _coroutines.Add(Coroutine);
        StartCoroutine(Coroutine);

        _audioSourcesManager.StartButtonClickUpEffect();
    }

    private IEnumerator WaitForPlayerToEnter()
    {
        yield return new WaitUntil(() => _enteredPlayer == true);
        _enteredPlayer = false;
        _buttonSprite.sprite = NotPressedButtonImage;
        ModifyColliderShape();

        StopCoroutines();
        IEnumerator Coroutine = _platformScript.ApplyStrategy(_movements[1]);
        _coroutines.Add(Coroutine);
        StartCoroutine(Coroutine);

        _audioSourcesManager.StartButtonClickUpEffect();
    }

    private void StopCoroutines()
    {
        foreach (IEnumerator Coroutine in _coroutines)
        {
            StopCoroutine(Coroutine);
        }
        _coroutines.Clear();
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