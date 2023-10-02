using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public Vector3 InitialPosition;

    // Variables for strategies:
    public float Distance;

    public float Speed;
    public float RotationTime;

    public bool GoingDown;

    // Start is called before the first frame update
    private void Start()
    {
        InitialPosition = transform.position;

        GoingDown = false;
    }

    public IEnumerator ApplyStrategy(IMovement strategy)
    {
        return strategy.Move(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (GoingDown)
            {
                collision.gameObject.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}