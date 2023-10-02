using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScript<T> : MonoBehaviour where T : Component
{
    private static T _scriptInstance;

    public static T ScriptInstance
    {
        get
        {
            if (_scriptInstance == null)
            {
                _scriptInstance = FindObjectOfType<T>();
                if (_scriptInstance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(T).Name;
                    _scriptInstance = go.AddComponent<T>();
                }
            }
            return _scriptInstance;
        }
    }

    private void Awake()
    {
        if (_scriptInstance == null)
        {
            _scriptInstance = this as T;
            // useful so that the instance of the object lives through all the scenes in the game:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}