using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            transform.SetParent(collision.gameObject.transform);
            transform.localPosition = new Vector3(-0.2f, 0.5f, 0f);
        }
    }
}