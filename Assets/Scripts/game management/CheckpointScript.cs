using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointScript : MonoBehaviour
{
    private BoxCollider2D _checkpointCollider;

    [SerializeField]
    private int _checkpointNumber;

    private GameObject _checkIcon;

    private AudioSourcesManager _audioSourcesManager;

    private TMP_Text _starsNumberText;

    private GameObject[] _hiddenStars;

    private GameObject[] _destructibleObjects;

    // Start is called before the first frame update
    private void Start()
    {
        _checkpointCollider = GetComponent<BoxCollider2D>();
        bool HasPrefs = CheckPlayerPrefs();
        _checkpointCollider.enabled = HasPrefs ? false : true;

        _checkIcon = transform.GetChild(0).gameObject;
        _checkIcon.SetActive(false);

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();

        _starsNumberText = GameObject.FindGameObjectWithTag("starsCanvas").transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>();

        _hiddenStars = GameObject.FindGameObjectsWithTag("hiddenStar");

        _destructibleObjects = GameObject.FindGameObjectsWithTag("destructibleObject");
    }

    private void Update()
    {
        bool HasPrefs = CheckPlayerPrefs();
        _checkpointCollider.enabled = HasPrefs ? false : true;
    }

    private bool CheckPlayerPrefs()
    {
        bool KeyOnX = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionX");
        bool KeyOnY = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionY");
        bool HasThrowKey = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "Throws");
        if (KeyOnX && KeyOnY && HasThrowKey)
        {
            return true;
        }
        return false;
    }

    private IEnumerator ShowCheckIcon()
    {
        _checkIcon.SetActive(true);
        _audioSourcesManager.StartCheckpointEffect();
        yield return new WaitForSeconds(2f);
        _checkIcon.SetActive(false);
    }

    private void AddHiddenStarsToPrefs()
    {
        foreach (GameObject Star in _hiddenStars)
        {
            HiddenStarScript StarScript = Star.GetComponent<HiddenStarScript>();
            if (StarScript._isCollected)
            {
                string HiddenStarNumber = Star.name[Star.name.Length - 1].ToString();
                PlayerPrefs.SetInt("Checkpoint" + _checkpointNumber.ToString() + "HiddenStar" + HiddenStarNumber, 1);
            }
        }
    }

    private void AddDestructibleObjectsToPrefs()
    {
        foreach (GameObject DestructibleObject in _destructibleObjects)
        {
            DestructibleObjectsScript DestructibleObjectScript = DestructibleObject.GetComponent<DestructibleObjectsScript>();
            if (DestructibleObjectScript._isDestroyed)
            {
                string DestructibleObjectNumber = DestructibleObject.name[DestructibleObject.name.Length - 1].ToString();
                PlayerPrefs.SetInt("Checkpoint" + _checkpointNumber.ToString() + "Destructible" + DestructibleObjectNumber, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _checkpointCollider.enabled = false;
            PlayerPrefs.SetFloat("Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionX", transform.position.x);
            PlayerPrefs.SetFloat("Checkpoint" + _checkpointNumber.ToString() + "PlayerPositionY", transform.position.y);

            int NumberOfThrows = int.Parse(_starsNumberText.text.Substring(2, _starsNumberText.text.Length - 2));
            PlayerPrefs.SetInt("Checkpoint" + _checkpointNumber.ToString() + "Throws", NumberOfThrows);

            AddHiddenStarsToPrefs();

            AddDestructibleObjectsToPrefs();

            StartCoroutine(ShowCheckIcon());
        }
    }
}