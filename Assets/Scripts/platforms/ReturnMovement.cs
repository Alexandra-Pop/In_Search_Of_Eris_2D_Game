using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMovement : MonoBehaviour, IMovement
{
    public IEnumerator Move(PlatformScript Platform)
    {
        IEnumerator Coroutine = ReturnToInitialPosition(Platform);
        return Coroutine;
    }

    public IEnumerator ReturnToInitialPosition(PlatformScript Platform)
    {
        Platform.GoingDown = Platform.transform.position.y > Platform.InitialPosition.y ? true : false;
        while (Platform.transform.position != Platform.InitialPosition)
        {
            transform.position = Vector2.MoveTowards(Platform.transform.position, Platform.InitialPosition, Time.deltaTime * Platform.Speed);
            yield return null;
        }
    }
}