using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : NetworkBehaviour
{

    int enemyHealth;
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    private Vector2 enemyPos;
    Animator animator;
    public enum EnemyState { patrol, seek};

    public EnemyState enemyState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyState = EnemyState.patrol;
    }

    // sync when they join
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.patrol:
                {
                    EnemyPosUpdateClientRPC(enemyPos);
                }
                break;
            case EnemyState.seek:
                {

                }
                break;
        }
    }

    [ClientRpc]
    private void EnemyPosUpdateClientRPC(Vector2 _enemyPos)
    {
        transform.position = _enemyPos;
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        transform.position = new Vector2(Mathf.PingPong(Time.time * speed, distance), 0);
        enemyPos = transform.position;
    }

    int flipCounter;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            enemyHealth++;
            animator.SetBool("IsSprint", true);
            if (enemyHealth == 2)
            {
               Destroy(gameObject);
            }
        }      
    }
}