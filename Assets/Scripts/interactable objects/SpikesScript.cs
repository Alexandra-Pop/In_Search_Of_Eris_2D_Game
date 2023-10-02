using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesScript : MonoBehaviour
{
    private MainCharacterScript _mainCharacterScript;

    // Start is called before the first frame update
    private void Start()
    {
        _mainCharacterScript = GameObject.FindGameObjectWithTag("throwableObjectPosition").GetComponentInParent<MainCharacterScript>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player collides with the spikes, it dies (IsDead == true):
        if (collision.gameObject.layer == 8)
        {
            _mainCharacterScript.IsDead = true;
        }
    }
}