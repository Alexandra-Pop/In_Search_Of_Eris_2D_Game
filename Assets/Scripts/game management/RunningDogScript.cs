using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningDogScript : MonoBehaviour
{
    private Rigidbody2D _dogRigidbody;
    private RectTransform _dogRectTransform;
    private float _movingSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        _dogRigidbody = GetComponent<Rigidbody2D>();
        _dogRectTransform = GetComponent<RectTransform>();
        _movingSpeed = 35;
    }

    // Update is called once per frame
    private void Update()
    {
        if ((_dogRectTransform.anchoredPosition.x >= -1100) && (_dogRectTransform.anchoredPosition.x < 1100))
        {
            _dogRigidbody.velocity = Vector2.right * _movingSpeed;
        }
        else
        {
            _dogRectTransform.anchoredPosition = new Vector2(-1100, _dogRectTransform.anchoredPosition.y);
        }
    }
}