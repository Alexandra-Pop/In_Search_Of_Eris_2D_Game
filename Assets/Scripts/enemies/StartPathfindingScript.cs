using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPathfindingScript : MonoBehaviour
{
    private EnemyScript _childEnemy;

    // Start is called before the first frame update
    private void Start()
    {
        _childEnemy = transform.GetChild(0).gameObject.GetComponent<EnemyScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 8) && !_childEnemy.Stunned)
        {
            _childEnemy.StartMoving();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _childEnemy.StopMoving();
        }
    }
}