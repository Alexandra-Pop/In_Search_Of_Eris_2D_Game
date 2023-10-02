using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionsScript : MonoBehaviour
{
    private Animator _levelTransition;

    public float TransitionTime;

    // Start is called before the first frame update
    private void Start()
    {
        _levelTransition = transform.GetChild(0).GetComponent<Animator>();

        TransitionTime = 1f;
    }

    public void LoadGivenScene(int Index)
    {
        StartCoroutine(LoadScene(Index));
    }

    private IEnumerator LoadScene(int Index)
    {
        _levelTransition.SetTrigger("EndLevel");
        yield return new WaitForSeconds(TransitionTime);
        SceneManager.LoadScene(Index);
    }

    public void StartSameLevelTransition()
    {
        StartCoroutine(StartTransition());
    }

    public IEnumerator StartTransition()
    {
        _levelTransition.SetTrigger("EndLevel");
        yield return new WaitForSeconds(TransitionTime);
        _levelTransition.SetTrigger("StartLevel");
    }
}