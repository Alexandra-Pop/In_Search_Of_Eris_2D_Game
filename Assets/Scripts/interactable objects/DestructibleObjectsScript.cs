using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleObjectsScript : MonoBehaviour
{
    private TilemapRenderer _destructibleTilemapRenderer;
    private ParticleSystem _particleSystemWhenDestory;

    [SerializeField]
    private int _checkpointNumber;

    public bool _isDestroyed;

    // Start is called before the first frame update
    private void Start()
    {
        _destructibleTilemapRenderer = GetComponent<TilemapRenderer>();
        _particleSystemWhenDestory = gameObject.GetComponent<ParticleSystem>();

        string DestructibleObjectNumber = gameObject.name[gameObject.name.Length - 1].ToString();
        bool ObjectState =
            PlayerPrefs.HasKey("Checkpoint" + _checkpointNumber.ToString() + "Destructible" + DestructibleObjectNumber) ? false : true;

        gameObject.SetActive(ObjectState);
        _isDestroyed = !ObjectState;
    }

    private void ParticlesEmission()
    {
        var Emission = _particleSystemWhenDestory.emission;
        Emission.enabled = true;
        var EmissionDuration = _particleSystemWhenDestory.main.duration;

        _destructibleTilemapRenderer.enabled = false;

        _particleSystemWhenDestory.Play();
        Invoke(nameof(DestroyObject), EmissionDuration);
    }

    private void DestroyObject()
    {
        gameObject.SetActive(false);
        _isDestroyed = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("throwableObject"))
        {
            ParticlesEmission();
        }
    }
}