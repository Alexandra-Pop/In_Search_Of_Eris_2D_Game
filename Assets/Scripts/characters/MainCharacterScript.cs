using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainCharacterScript : MonoBehaviour
{
    private Rigidbody2D _characterRigidbody;
    private float _movingVelocity;
    private float _horizontalPosition;

    private bool _onTheGround;
    private float _characterRightXPosition;

    private float _jumpingHeight;

    private float _jumpingGravity;
    private float _fallingGravity;

    private bool _canGrab;
    private bool _isGrabbing;
    private Rigidbody2D _grabbedObject;

    private bool _canDash;
    private bool _playerIsDashing;
    private float _dashingVelocity;
    private float _dashingDuration;
    private TrailRenderer _dashingTrail;

    private GameOverScript _gameOver;

    private int _possibleThrows;
    private float _verticalThrowVelocity;
    private float _horizontalThrowVelocity;
    private bool _canThrow;
    public GameObject _throwableObject;
    private Transform _throwPosition;

    private Animator _playerAnimator;
    private bool _isJumping;
    private bool _isFalling;

    private enum _playerState
    { idle, run, jump, fall, hurt };

    public bool IsDead;

    public bool CanMove;

    private TMP_Text _starsNumberText;

    private AudioSourcesManager _audioSourcesManager;

    [SerializeField]
    private int _checkpointNumber;

    public Vector2 InitialPosition;

    [SerializeField]
    private int _initialPossibleThrows;

    // Start is called before the first frame update
    private void Start()
    {
        _characterRigidbody = GetComponent<Rigidbody2D>();
        CheckPlayerPosition();

        _movingVelocity = 15;
        _jumpingHeight = 7;
        _onTheGround = true;
        _characterRightXPosition = transform.localScale.x;

        _jumpingGravity = 10;
        _fallingGravity = 15;
        _characterRigidbody.gravityScale = _jumpingGravity;

        _canGrab = false;
        _isGrabbing = false;
        _grabbedObject = null;

        _canDash = false;
        _playerIsDashing = false;
        _dashingVelocity = 30f;
        _dashingDuration = 0.3f;
        _dashingTrail = GetComponent<TrailRenderer>();

        _gameOver = GameObject.FindGameObjectWithTag("gameOverScript").GetComponent<GameOverScript>();

        CheckPossibleThrows();
        _verticalThrowVelocity = 10f;
        _horizontalThrowVelocity = 20f;
        _canThrow = true;
        _throwPosition = GameObject.FindGameObjectWithTag("throwableObjectPosition").transform;

        _playerAnimator = GetComponent<Animator>();
        _isJumping = false;
        _isFalling = false;

        IsDead = false;

        CanMove = true;

        _starsNumberText = GameObject.FindGameObjectWithTag("starsCanvas").transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>();
        _starsNumberText.text = "x " + _possibleThrows.ToString();

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (CanMove)
        {
            MovingLeftAndRight();
            Jumping();
            ChangeJumpingGravity();
            GrabObjects();
            StartCoroutine(Dash());
            Throw();
            ModifyAnimationState();
        }
    }

    private void MovingLeftAndRight()
    {
        // Move left and right: foloseste axa x (in project settings exista elementul Horizontal care preia comanda de la sagetile st. si dr.
        // sau tastele a si d) si o viteza pentru a misca caracterul la st. si dr.;
        // circle collider >>> box collider (asa caracterul nu se blocheaza in obiecte, mai ales nu se blocheaza in ground);
        // caracterul are rotation pe z freeze ca sa nu se roteasca cand se misca;

        // De adaugat in viitor: partea in care daca te lovesti de perete si iti folosesti jump-ul si iar te lovesti de perete sa nu
        // mai poti merge inspre perete si sa ramai in aer, deci sa cazi;

        // Time.timeScale == 1 verificat ca sa stim daca jocul e pus pe pauza sau nu;
        // Player can move left and right if timeScale is 1 and if it is not dashing:
        if (Time.timeScale == 1 && !_playerIsDashing)
        {
            _horizontalPosition = Input.GetAxis("Horizontal");
            _characterRigidbody.velocity = new Vector2(_horizontalPosition * _movingVelocity, _characterRigidbody.velocity.y);
            // Move the throw target along with the player:
            // MoveThrowTarget();  -> daca merge metoda noua nu il mai folosesc;

            // Character flip when moving - initial position is to the right (if the character isn't moving, we are leaving it in the last
            // position - when _horizontalPosition is 0, the character is stationary so we don't do anything);
            // if we grab an object (_isGrabbing), we are not flipping the character so it looks like it drags or pushes the object:
            if ((_horizontalPosition < 0f) && !_isGrabbing)
            {
                // flip to left:
                transform.localScale = new Vector3(-_characterRightXPosition, transform.localScale.y, transform.localScale.z);
            }
            else if ((_horizontalPosition > 0f) && !_isGrabbing)
            {
                // flip to right:
                transform.localScale = new Vector3(_characterRightXPosition, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void Jumping()
    {
        // jumping - Time.timeScale == 1 - we verify it to know if the game is paused or not (so we allow the movement or not);
        // the player can jump once if it touches the ground or if it touches a grabbable object (the _canGrab flag is set to false
        // in the onCollisionExit method), and if it is not dashing:
        if (Input.GetKeyDown(KeyCode.Space) && (_onTheGround || _canGrab) && (Time.timeScale == 1) && !_playerIsDashing)
        {
            _isJumping = true;
            _isFalling = false;
            _characterRigidbody.gravityScale = _jumpingGravity;
            float JumpingVelocity = Mathf.Sqrt(_jumpingHeight * _characterRigidbody.gravityScale * Physics2D.gravity.y * -2) * _characterRigidbody.mass;
            _characterRigidbody.AddForce(Vector2.up * JumpingVelocity, ForceMode2D.Impulse);

            _audioSourcesManager.StartMainCharacterJumpEffect();
        }
    }

    private void ChangeJumpingGravity()
    {
        // increase the gravity when falling (after a jump) - if the player is not dashing (when we dash, we want the gravity set to 0 for
        // the _dashingDuration time):
        if ((Mathf.Sign(_characterRigidbody.velocity.y) == -1) && !_playerIsDashing)
        {
            _characterRigidbody.gravityScale = _fallingGravity;
            // if character is not on the ground and character isn't touching grabbable objects, and the velocity is < 0, then it is falling:
            if (!_onTheGround && !_canGrab)
            {
                _isJumping = false;
                _isFalling = true;
            }
        }
        else if (!_playerIsDashing)
        {
            _characterRigidbody.gravityScale = _jumpingGravity;
        }
    }

    private void GrabObjects()
    {
        // while the "Z" key is pressed, the object that collided with our character moves with us (the touched object is set from freeze on x axis
        // to not freeze on x axis), if the player isn't dashing (we don't want to dash while moving objects):
        if (Input.GetKey(KeyCode.Z) && _canGrab && _onTheGround && !_playerIsDashing && (Time.timeScale == 1))
        {
            _isGrabbing = true;
            _grabbedObject.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            _grabbedObject.velocity = new Vector2(_horizontalPosition * _movingVelocity, _grabbedObject.velocity.y);
        }

        // if we let go of the "Z" key, we change the _isGrabbing flag to false, and the object to freeze on x axis and z axis;
        // we do this because before, we changed these values only when exiting the collision, so we could only press "Z" once and still
        // move the object forward for some time; The problem was that when pressing "Z", the object changed to not freeze on x axis, and if
        // we didn't exit the collision we wouldn't change it back to freeze on x axis and we could move the dynamic object;
        // To solve the problem, when touching the object, if we let go of the "Z" key, we make the object freeze again, and disable the
        // grabbing action;
        // I added the _gabbedObject != null verification because the OnCollisionExit method can be triggered before setting the object to
        // freeze on x axis, and that results in setting the null object to freeze on x axis, and that raises an error:
        if (Input.GetKeyUp(KeyCode.Z) && _grabbedObject != null)
        {
            _isGrabbing = false;
            _grabbedObject.constraints = RigidbodyConstraints2D.FreezeAll;
            _grabbedObject.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        }
    }

    private IEnumerator Dash()
    {
        // the player can dash if the "_canDash" flag is set to true (the flag is set to true in the OnCollisionEnter method - when we touch
        // the ground or a GrabbableObject) and if it isn't grabbing a "GrabbableObject"; the _canDash flag is set back to false when we execute
        // the dash, and set back to true if we are back on the ground (so we can continously dash on the ground):
        float HorizontalControl = Input.GetAxis("Horizontal");
        float VerticalControl = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.X) && _canDash && !_isGrabbing && (Time.timeScale == 1))
        {
            _canDash = false;
            _playerIsDashing = true;
            _characterRigidbody.gravityScale = 0f;
            _dashingTrail.emitting = true;

            Vector2 DashingDirection = new Vector2(HorizontalControl, VerticalControl);

            // localScale helps with the direction of the character when we don't press Left/Right Arrow:
            if (DashingDirection == Vector2.zero)
            {
                DashingDirection = new Vector2(transform.localScale.x * _dashingVelocity, 0f);
            }
            // if we want to dash on diagonal, we set the controls to the sign of them (1/-1), so when we normalize the, we will always
            // go on the same angle on diagonal (values after normalization: 0.71/-0.71):
            else if ((DashingDirection.x != 0) && (DashingDirection.y != 0))
            {
                DashingDirection = new Vector2(Mathf.Sign(DashingDirection.x), Mathf.Sign(DashingDirection.y));
            }

            // The ".normalized" makes the length of the vector = to 1/-1; So x + y values = 1/-1; So we always have 1/-1 * the power of dash:
            _characterRigidbody.velocity = DashingDirection.normalized * _dashingVelocity;

            _audioSourcesManager.StartMainCharacterDashEffect();

            yield return new WaitForSeconds(_dashingDuration);

            // if we dash directly on the ground, the OnCollisionEnter method does not trigger, so we set the _canDash flag to true so we
            // can continue to dash;
            // if we dash in the air, the OnCollisionExit method will trigger, and the _canDash flag won't be set to true here, so we cannot
            // dash multiple times in the air; but when touching the ground, the OnCollisionEnter will trigger, and the _candash flag will be set
            // to true, so we can dash again:
            if (_onTheGround)
            {
                _canDash = true;
            }

            _characterRigidbody.gravityScale = _jumpingGravity;
            _playerIsDashing = false;
            _dashingTrail.emitting = false;
        }
    }

    private bool CanThrow()
    {
        if (_isGrabbing || _playerIsDashing)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Throw()
    {
        _canThrow = CanThrow();

        // throw stone with an arch:
        if (Input.GetKeyDown(KeyCode.C) && _canThrow && (_possibleThrows > 0) && (Time.timeScale == 1))
        {
            _canThrow = false;

            GameObject StoneToThrow = Instantiate(_throwableObject, _throwPosition.position, transform.rotation);
            Rigidbody2D StoneToThrowRigidbody = StoneToThrow.GetComponent<Rigidbody2D>();

            Vector2 Force = Mathf.Sign(transform.localScale.x) * Vector2.right * _horizontalThrowVelocity + Vector2.up * _verticalThrowVelocity;
            StoneToThrowRigidbody.AddForce(Force, ForceMode2D.Impulse);

            _possibleThrows--;
            _starsNumberText.text = "x " + _possibleThrows.ToString();
        }
        // throw object horizontally:
        else if (Input.GetKeyDown(KeyCode.V) && _canThrow && (_possibleThrows > 0))
        {
            _canThrow = false;

            GameObject StoneToThrow = Instantiate(_throwableObject, _throwPosition.position, transform.rotation);
            Rigidbody2D StoneToThrowRigidbody = StoneToThrow.GetComponent<Rigidbody2D>();
            StoneToThrowRigidbody.gravityScale = 0;

            Vector2 Force = Mathf.Sign(transform.localScale.x) * Vector2.right * _horizontalThrowVelocity;

            StoneToThrowRigidbody.AddForce(Force, ForceMode2D.Impulse);

            _possibleThrows--;
            _starsNumberText.text = "x " + _possibleThrows.ToString();
        }
    }

    private void ModifyAnimationState()
    {
        _playerState AnimationState;

        if (_horizontalPosition != 0f)
        {
            AnimationState = _playerState.run;
        }
        else
        {
            AnimationState = _playerState.idle;
        }

        if (_isJumping || _playerIsDashing)
        {
            AnimationState = _playerState.jump;
        }
        else if (_isFalling)
        {
            AnimationState = _playerState.fall;
        }

        if (IsDead)
        {
            AnimationState = _playerState.hurt;
        }

        _playerAnimator.SetInteger("animationState", (int)AnimationState);
        if (IsDead)
        {
            StartCoroutine(PlayGameOverScreen());
        }
    }

    private IEnumerator PlayGameOverScreen()
    {
        float DeadAnimationTime = _playerAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(DeadAnimationTime);
        _gameOver.GameOver();
    }

    public void AddStarToItems(int Value)
    {
        _possibleThrows += Value;
        _starsNumberText.text = "x " + _possibleThrows.ToString();
    }

    public void CheckPlayerPosition()
    {
        string CheckpointKeyX = "Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionX";
        string CheckpointKeyY = "Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionY";
        bool KeyOnX = PlayerPrefs.HasKey(CheckpointKeyX);
        bool KeyOnY = PlayerPrefs.HasKey(CheckpointKeyY);
        if (KeyOnX && KeyOnY)
        {
            float XPos = PlayerPrefs.GetFloat(CheckpointKeyX);
            float YPos = PlayerPrefs.GetFloat(CheckpointKeyY);
            Vector2 newPosition = new Vector2(XPos, YPos);
            _characterRigidbody.position = newPosition;
        }
        else
        {
            _characterRigidbody.position = InitialPosition;
        }
    }

    public void CheckPossibleThrows()
    {
        string CheckpointThrows = "Checkpoint" + _checkpointNumber.ToString() + "Throws";
        bool HasKey = PlayerPrefs.HasKey(CheckpointThrows);
        if (HasKey)
        {
            _possibleThrows = PlayerPrefs.GetInt(CheckpointThrows);
        }
        else
        {
            _possibleThrows = _initialPossibleThrows;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _onTheGround = true;
            _canDash = true;

            // if character is on the ground, it is neither jumping, nor falling:
            _isJumping = false;
            _isFalling = false;
        }

        // if the collision object is a "GrabbableObject", we set the "_canGrab" flag to true and set the object variable to the collided object,
        // so we can move it; we also want it to freeze on x axis so it moves only when touched and "Z" key is pressed:
        if (collision.gameObject.layer == 6)
        {
            _canGrab = true;
            _grabbedObject = collision.collider.attachedRigidbody;
            _grabbedObject.constraints = RigidbodyConstraints2D.FreezeAll;
            _grabbedObject.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

            _canDash = true;

            // if character touches the grabbable object, it is neither jumping, nor falling:
            _isJumping = false;
            _isFalling = false;
        }

        // the game over logic - if the character touches the edge collision 2D that sits under the entire level, then the level it's over,
        // and the player has to start over again (the layer is 9 = GameOver):
        if (collision.gameObject.layer == 9)
        {
            _gameOver.GameOver();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // when we don't touch a "GrabbableObject", we set the "_canGrab" flag to false and set the collided object variable to null,
        // so we can't move it anymore; we also reset the object to freeze on x axis, and we set the "_isGrabbing" flag to false, because that
        // action is stopped (helps in the dashing method):
        if (collision.gameObject.layer == 6)
        {
            _grabbedObject.constraints = RigidbodyConstraints2D.FreezeAll;
            _grabbedObject.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            _canGrab = false;
            _isGrabbing = false;
            _grabbedObject = null;
        }

        // when we rise from the ground, we set the flag to false (used for jumping, dashing and grabbing objects):
        if (collision.gameObject.layer == 3)
        {
            _onTheGround = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // if we stay on the ground and the previous state was not on the ground, we set the needed flags to true:
        if (collision.gameObject.layer == 3 && !_onTheGround)
        {
            _onTheGround = true;
            _canDash = true;

            // if character is on the ground, it is neither jumping, nor falling:
            _isJumping = false;
            _isFalling = false;
        }
    }
}