using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    public IEnumerator Move(PlatformScript Platform);
}