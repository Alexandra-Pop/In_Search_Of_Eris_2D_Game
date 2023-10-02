using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObjectScript : MonoBehaviour
{
    private SpriteRenderer _throwableObjectSprite;
    private Rigidbody2D _throwableObjectRigidbody;
    private ParticleSystem _particleSystemWhenDestory;
    private float _instantiatedTime;
    private float _timeToLive;
    private bool _isHorizontal;
    private bool _timeEnded;
    private bool _collided;

    private AudioSourcesManager _audioSourcesManager;

    // Start is called before the first frame update
    private void Start()
    {
        _throwableObjectSprite = gameObject.GetComponent<SpriteRenderer>();
        _throwableObjectRigidbody = gameObject.GetComponent<Rigidbody2D>();
        _particleSystemWhenDestory = gameObject.GetComponent<ParticleSystem>();
        _instantiatedTime = Time.time;
        _timeToLive = 0.5f;
        if (_throwableObjectRigidbody.velocity.y == 0)
        {
            _isHorizontal = true;
        }
        else
        {
            _isHorizontal = false;
        }
        _timeEnded = false;
        _collided = false;

        _audioSourcesManager = GameObject.FindGameObjectWithTag("audioSourcesManager").GetComponentInParent<AudioSourcesManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        float Diff = Mathf.Abs(_instantiatedTime - Time.time);
        if ((Diff > _timeToLive) && _isHorizontal && !_timeEnded && !_collided)
        {
            _timeEnded = true;
            _audioSourcesManager.StartStarDissapearsEffect();
            ParticlesEmission();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("Pozitie dupa aruncare fara obstacole: " + transform.position);
        if (!_timeEnded)
        {
            // Debug.Log("Collided with: " + collision.gameObject.tag);
            _collided = true;
            _audioSourcesManager.StartStarHitsObjectEffect();
            ParticlesEmission();
        }
    }

    private void ParticlesEmission()
    {
        var Emission = _particleSystemWhenDestory.emission;
        Emission.enabled = true;

        var EmissionDuration = _particleSystemWhenDestory.main.duration;

        Destroy(_throwableObjectSprite);
        _throwableObjectRigidbody.bodyType = RigidbodyType2D.Static;

        _particleSystemWhenDestory.Play();
        Invoke(nameof(DestroyThrowableObject), EmissionDuration);
    }

    private void DestroyThrowableObject()
    {
        Destroy(gameObject);
    }
}