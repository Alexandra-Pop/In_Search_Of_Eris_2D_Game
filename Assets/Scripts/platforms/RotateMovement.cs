using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMovement : MonoBehaviour, IMovement
{
    public IEnumerator Move(PlatformScript Platform)
    {
        IEnumerator Coroutine = RotateLeftOrRight(Platform);
        return Coroutine;
    }

    public IEnumerator RotateLeftOrRight(PlatformScript Platform)
    {
        float CurrentTime = Time.time;
        float Diff = Mathf.Abs(CurrentTime - Time.time);
        Vector3 PlatformPivot = Platform.transform.GetChild(0).transform.position;

        while (Diff < Platform.RotationTime)
        {
            Platform.transform.RotateAround(PlatformPivot, Vector3.forward, Platform.Speed * Time.deltaTime);
            Diff = Mathf.Abs(CurrentTime - Time.time);
            yield return null;
        }
        Platform.transform.rotation = Quaternion.identity;
    }
}