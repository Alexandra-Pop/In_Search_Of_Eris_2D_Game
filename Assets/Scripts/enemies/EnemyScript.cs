using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    private AIPath _path;

    private float _enemyInitialDirection;

    private MainCharacterScript _mainCharacterScript;

    public bool Stunned;

    // Start is called before the first frame update
    private void Start()
    {
        _path = gameObject.GetComponent<AIPath>();
        // initially, the enemy isn't moving, till the destination (the character) enters the enemy zone (the pathfinding grid):
        StopMoving();
        _enemyInitialDirection = transform.localScale.x;

        _mainCharacterScript = GameObject.FindGameObjectWithTag("throwableObjectPosition").GetComponentInParent<MainCharacterScript>();

        Stunned = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // flip the enemy according to velocity (same logic as the main character flip, but reversed because the initial sprite faces left):
        Vector2 EnemyDesiredDirection = _path.desiredVelocity;
        if (EnemyDesiredDirection.x > 0f)
        {
            transform.localScale = new Vector3(-_enemyInitialDirection, transform.localScale.y, transform.localScale.z);
        }
        else if (EnemyDesiredDirection.x < 0f)
        {
            transform.localScale = new Vector3(_enemyInitialDirection, transform.localScale.y, transform.localScale.z);
        }
    }

    public void StartMoving()
    {
        _path.canMove = true;
    }

    public void StopMoving()
    {
        _path.canMove = false;
    }

    private IEnumerator StartStun()
    {
        Stunned = true;
        StopMoving();
        GameObject StunEffect = gameObject.transform.GetChild(0).gameObject;
        StunEffect.SetActive(true);
        yield return new WaitForSeconds(5f);
        StunEffect.SetActive(false);
        StartMoving();
        Stunned = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player collides with the enemy, it dies (IsDead == true):
        if (collision.gameObject.layer == 8)
        {
            _mainCharacterScript.IsDead = true;
        }

        // if the enemy is inside a puzzle and if it is hit by a star, it gets stunned:
        if (gameObject.CompareTag("stunEnemy") && collision.gameObject.CompareTag("throwableObject"))
        {
            StartCoroutine(StartStun());
        }
    }
}