using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantMovement : MonoBehaviour, IMovement
{
    public IEnumerator Move(PlatformScript Platform)
    {
        MoveToPosition(Platform);
        return null;
    }

    public void MoveToPosition(PlatformScript Platform)
    {
        Platform.transform.position = new Vector2(Platform.transform.position.x, Platform.transform.position.y + Platform.Distance);
    }
}