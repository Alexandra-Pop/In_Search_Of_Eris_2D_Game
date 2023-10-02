using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenStarScript : MonoBehaviour
{
    private MainCharacterScript _mainCharacterScript;

    private AudioSourcesManager _audioSourcesManager;

    [SerializeField]
    private int _checkpointNumber;

    public bool _isCollected;

    [SerializeField]
    private int _nbOfStars;

    // Start is called before the first frame update
    private void Start()
    {
        string HiddenStarNumber = gameObject.name[gameObject.name.Length - 1].ToString();
        bool ObjectState = PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "HiddenStar" + HiddenStarNumber) ? false : true;
        // Debug.Log("Verifying star " + "Checkpoint" + _checkpointNumber.ToString() + "HiddenStar" + HiddenStarNumber);
        // Debug.Log("Star state " + ObjectState);
        // Debug.Log("Is collected " + !ObjectState);
        gameObject.SetActive(ObjectState);
        _isCollected = !ObjectState;

        _mainCharacterScript = GameObject.FindGameObjectWithTag("throwableObjectPosition").GetComponentInParent<MainCharacterScript>();

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            gameObject.SetActive(false);
            _mainCharacterScript.AddStarToItems(_nbOfStars);
            _audioSourcesManager.StartStarCollectingEffect();

            _isCollected = true;
        }
    }
}