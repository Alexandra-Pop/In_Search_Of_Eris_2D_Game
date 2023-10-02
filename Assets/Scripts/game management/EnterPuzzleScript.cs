using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnterPuzzleScript : MonoBehaviour
{
    private CinemachineVirtualCamera _gameCamera;

    [SerializeField]
    private float _cameraDistance;

    // Start is called before the first frame update
    private void Start()
    {
        _gameCamera = GameObject.FindGameObjectWithTag("followCamera").GetComponent<CinemachineVirtualCamera>();
        _gameCamera.m_Lens.OrthographicSize = 12.1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _gameCamera.m_Lens.OrthographicSize = _cameraDistance;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _gameCamera.m_Lens.OrthographicSize = 12.1f;
        }
    }
}