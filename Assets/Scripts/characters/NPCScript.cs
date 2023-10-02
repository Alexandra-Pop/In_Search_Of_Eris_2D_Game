using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCScript : MonoBehaviour
{
    public Transform PlayerPosition;
    public MainCharacterScript _mainCharacterScript;

    private GameObject _npcArrow;
    private GameObject _npcText;

    private Canvas _dialoguePanel;
    private TMP_Text _dialogueText;
    private TMP_Text _nextLineText;
    private TMP_Text _exitDialogueText;

    [SerializeField]
    private string[] _dialogueLines;

    private int _index;
    private float _lettersSpeed;
    private bool _canCommunicate;
    private bool _finishedDialogue;
    private bool _isCommunicating;

    private AudioSource _gibberishTalk;

    [SerializeField]
    private AudioClip[] _gibberishSounds;

    private int _audioFrequency;
    private float _minPitch;
    private float _maxPitch;

    // Start is called before the first frame update
    private void Start()
    {
        _npcArrow = GameObject.FindGameObjectWithTag("npc1Arrow");
        _npcArrow.SetActive(false);

        _npcText = GameObject.FindGameObjectWithTag("npcConvoInfo");
        _npcText.SetActive(false);

        _dialoguePanel = transform.GetChild(2).gameObject.GetComponent<Canvas>();
        _dialoguePanel.enabled = false;
        _dialogueText = _dialoguePanel.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        _nextLineText = _dialoguePanel.transform.GetChild(0).transform.GetChild(3).gameObject.GetComponent<TMP_Text>();
        _nextLineText.enabled = false;
        _exitDialogueText = _dialoguePanel.transform.GetChild(0).transform.GetChild(4).gameObject.GetComponent<TMP_Text>();
        _exitDialogueText.enabled = false;
        /*
        _dialogueLines = new string[] { "Hello, welcome to the game where you have to find your dog named Eris",
            "I'm Hooded Man and I'm here to help you get used to this world!",
            "First of all, you can move left and right with the left/right arrows or a/d keys, whichever you like more",
            "You can jump pressing the space bar",
            "To reach far away platforms you can dash in 8 directions by pressing x key and the direction that you want",
            "Some ojects are grabbable and you can move them by holding the z key"};
        */
        _index = 0;
        _lettersSpeed = 0.04f;
        _canCommunicate = false;
        _finishedDialogue = false;
        _isCommunicating = false;

        // Need this for stopping the character from moving when it is interacting with the npc:
        _mainCharacterScript = PlayerPosition.GetComponent<MainCharacterScript>();

        _gibberishTalk = GetComponent<AudioSource>();
        _audioFrequency = 3;
        _minPitch = 0.5f;
        _maxPitch = 1.5f;
    }

    // Update is called once per frame
    private void Update()
    {
        // Flip npc according to player position; the npc is originally facing right, so when the player is on its left, the npc should
        // have the direction with -Abs(localscale), and when the player is on its right, it should have the direction Abs(localscale);
        // The logic is like the other flipping logic, but written differently; Also flip the text above the npc (the text is set in world
        // space and moved above the character) whenever the npc flips because the text is a child of the npc object, and even if the text
        // doesn't flip on its own, it flips when its parent flips:
        if (PlayerPosition.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _npcText.transform.localScale = new Vector3(-Mathf.Abs(_npcText.transform.localScale.x), _npcText.transform.localScale.y,
                _npcText.transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _npcText.transform.localScale = new Vector3(Mathf.Abs(_npcText.transform.localScale.x), _npcText.transform.localScale.y,
                _npcText.transform.localScale.z);
        }

        Communicate();
    }

    private void PlayGibberishSound(int Index)
    {
        if (Index % _audioFrequency == 0)
        {
            _gibberishTalk.Stop();
            _gibberishTalk.pitch = Random.Range(_minPitch, _maxPitch);
            AudioClip ClipToPlay = _gibberishSounds[Random.Range(0, _gibberishSounds.Length)];
            Debug.Log("Playing sound clip no: " + ClipToPlay.name);
            _gibberishTalk.PlayOneShot(ClipToPlay);
        }
    }

    // Shows the line letter by letter, with a small delay between them, and sets the text that tells you to press enter to continue to
    // showing, and the _isCommunication (that tells us the npc finished its current line; is needed when we want to enable or disable
    // the player from exiting the dialogue with the keypadEnter key) to false:
    private IEnumerator LineTyping()
    {
        foreach (char letter in _dialogueLines[_index].ToCharArray())
        {
            PlayGibberishSound(_dialogueText.text.Length);
            _dialogueText.text += letter;
            yield return new WaitForSeconds(_lettersSpeed);
        }
        _gibberishTalk.Stop();
        _nextLineText.enabled = true;
        _exitDialogueText.enabled = true;
        _isCommunicating = false;
    }

    private void GoToNextLine()
    {
        if (_index < _dialogueLines.Length)
        {
            _dialogueText.text = "";
            StartCoroutine(LineTyping());
            _index++;
        }
        else
        {
            // If the user pressed "Enter" and the npc has no more lines, the dialogue canvas is set to hidden, and the character can move
            // again:
            _dialogueText.text = "";
            _dialoguePanel.enabled = false;
            _finishedDialogue = true;
            _mainCharacterScript.CanMove = true;
        }
    }

    private void Communicate()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _canCommunicate)
        {
            // If the npc talks, we can't do the nextLine part, but we can make it display the full line and stop talking(_gibberishTalk.Stop()),
            // so the player can't mess up the dialogue (we do this by checking the _isCommunicating flag, that is set to true when pressing
            // enter and set to false when the npc ends its line or the player re-clicks the enter key):
            if (!_isCommunicating)
            {
                // When we interact with the npc, the main character can't move anymore (by setting its public variable CanMove from its script
                // to false, and then back to true):
                _mainCharacterScript.CanMove = false;
                // This variable tells us that the npc is in the middle of saying its line:
                _isCommunicating = true;
                // If the arrow and "press enter" text above the npc are visible, after pressing enter to communicate with the npc, hide them:
                if (_npcArrow.activeSelf && _npcText.activeSelf)
                {
                    _npcArrow.SetActive(false);
                    _npcText.SetActive(false);
                }
                //If it is the first time pressing the enter key to communicate, you have to make the canvas showable:
                if (!_dialoguePanel.isActiveAndEnabled)
                {
                    _dialoguePanel.enabled = true;
                }

                // Hide the "<Enter> to continue" and "<Keypad enter> to exit" texts and make them visible again when the line ends showing:
                _nextLineText.enabled = false;
                _exitDialogueText.enabled = false;
                GoToNextLine();
            }
            else
            {
                StopAllCoroutines();
                _gibberishTalk.Stop();
                _nextLineText.enabled = true;
                _exitDialogueText.enabled = true;
                _isCommunicating = false;
                _dialogueText.text = _dialogueLines[_index - 1];
            }
        }

        // If we press this key, we hide the dialogue canvas, and let the main character move again (if the npc doesn't have any other line,
        // we say that the dialogue is finished -> _finishedDialogue to true):
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && _canCommunicate && !_isCommunicating)
        {
            if (_index < _dialogueLines.Length - 1)
            {
                _npcArrow.SetActive(true);
                _npcText.SetActive(true);
            }
            else
            {
                _canCommunicate = false;
                _finishedDialogue = true;
            }

            _dialogueText.text = "";
            _dialoguePanel.enabled = false;
            _mainCharacterScript.CanMove = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Show info only if player triggers this method, and only if the dialogue is not finished:
        if (collision.gameObject.layer == 8)
        {
            if (!_finishedDialogue)
            {
                _npcArrow.SetActive(true);
                _npcText.SetActive(true);
                _canCommunicate = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Hide info only if player triggers this method:
        if (collision.gameObject.layer == 8)
        {
            _npcArrow.SetActive(false);
            _npcText.SetActive(false);
            _canCommunicate = false;
        }
    }
}