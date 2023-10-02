using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : MonoBehaviour, IMovement
{
    public IEnumerator Move(PlatformScript Platform)
    {
        IEnumerator Coroutine = MoveUpOrDown(Platform);
        return Coroutine;
    }

    private IEnumerator MoveUpOrDown(PlatformScript Platform)
    {
        Vector2 Target = new Vector2(Platform.transform.position.x, Platform.InitialPosition.y + Platform.Distance);
        while (Platform.transform.position.y != Target.y)
        {
            Platform.transform.position = Vector2.MoveTowards(Platform.transform.position, Target, Time.deltaTime * Platform.Speed);
            yield return null;
        }
    }
}