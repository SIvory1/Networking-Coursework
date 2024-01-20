using System.Collections;
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
    SpriteRenderer sprite;
    Rigidbody2D rb;

    public enum EnemyState { patrol, seek};
    public EnemyState enemyState;

    GameObject[] _player;
    private float[] distanceFromPlayer;
    Vector3 closetPlayer;

    Vector2 desiredVelocity;
    Vector3 steeringVelocity;

    bool isAgro;

    float currentX;
    float priorX;

    // when player shoots get the player and then do everything

    private void Start()
    {
        _player = GameObject.FindGameObjectsWithTag("Player");
        print(_player.Length);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        enemyState = EnemyState.patrol;
        distanceFromPlayer = new float[_player.Length];

        isAgro = true;

        priorX = transform.position.x;
    }

    // sync when they join
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            enemyState = EnemyState.seek;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            enemyState = EnemyState.patrol;
        }

        switch (enemyState)
        {
            case EnemyState.patrol:
                {
                    EnemyPosUpdateClientRPC(enemyPos, isAgro, currentX, priorX);
                }
                break;
            case EnemyState.seek:
                {
                    FindClosestPlayerClientRpc();
                    SeekClientRpc(enemyPos, closetPlayer);
                }
                break;
        }
    }

    [ClientRpc]
    public void SeekClientRpc(Vector3 _enemyPos, Vector3 _closetPlayer)
    {
        // Get the desired velocity for seek and limit to maxSpeed
        desiredVelocity = Vector3.Normalize(_closetPlayer - _enemyPos) * speed;

        // Calculate steering velocity
        steeringVelocity = desiredVelocity - rb.velocity;

        transform.position += steeringVelocity * Time.deltaTime;

        enemyPos = transform.position;
    }


    [ClientRpc]
    void FindClosestPlayerClientRpc()
    {

        for (int i = 0; i < _player.Length; i++)
        {
            if (_player[i] != null)
            {
                distanceFromPlayer[i] = (_player[i].transform.position-transform.position).magnitude;
            }
        }

        if (_player.Length > 1)
        {
            if (distanceFromPlayer[0] < distanceFromPlayer[1])
            {
                closetPlayer = _player[0].transform.position;
            }
            else
            {
                closetPlayer = _player[1].transform.position;
            }
        }
        else
        {
            closetPlayer = _player[0].transform.position;
        }
        //float min = distanceFromPlayer.Min();    
    }

    [ClientRpc]
    private void EnemyPosUpdateClientRPC(Vector3 _enemyPos, bool _isAgro, float _currentX, float _priorX)
    {
        if (_isAgro)
        {
            transform.position = _enemyPos;
            MoveEnemy();
            SpriteFlip(_currentX, _priorX);

        }
    }

    private void SpriteFlip(float _currentX, float _priorX)
    {
        if (_priorX > _currentX)
        {
            sprite.flipX = true;
            priorX = _currentX;
            currentX = transform.position.x;
        }
        else
        {
            sprite.flipX = false;
            priorX = _currentX;
            currentX = transform.position.x;
        }
    }

    private void MoveEnemy()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time * speed, distance), -1.6f, 0);
        enemyPos = transform.position;     
    }

    int flipCounter;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            enemyHealth++;         
            if (enemyHealth == 2)
            {
                isAgro = false;
                animator.SetBool("TakenDMG", true);
                StartCoroutine(DestroyEnemy());
            }
        }      
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        DestroyEnemyClientRpc();
    }

    [ClientRpc]
    void DestroyEnemyClientRpc()
    {
        Destroy(gameObject);
    }
}