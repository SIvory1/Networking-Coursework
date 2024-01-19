using Unity.Netcode;
using UnityEngine;


public class BasicEnemy : NetworkBehaviour
{

    int enemyHealth;
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    private Vector2 enemyPos;
    Animator animator;
    Rigidbody2D rb;

    public enum EnemyState { patrol, seek};
    public EnemyState enemyState;

    GameObject[] _player;
    private float[] distanceFromPlayer;

    private void Start()
    {
        _player = GameObject.FindGameObjectsWithTag("Player");
        print(_player.Length);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyState = EnemyState.patrol;
        distanceFromPlayer = new float[_player.Length];

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
                    FindClosestPlayer();
                    Seek();
                }
                break;
        }   

    }

    Vector2 desiredVelocity;
    Vector3 steeringVelocity;
    public void Seek()
    {
        // Get the desired velocity for seek and limit to maxSpeed
        desiredVelocity = Vector3.Normalize(closetPlayer.transform.position - transform.position) * speed;

        // Calculate steering velocity
        steeringVelocity = desiredVelocity - rb.velocity;

        transform.position += steeringVelocity * Time.deltaTime;
    }

    GameObject closetPlayer;

    void FindClosestPlayer()
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
                closetPlayer = _player[0];
            }
            else
            {
                closetPlayer = _player[1];
            }
        }
        else
        {
            closetPlayer = _player[0];
        }
 
        //float min = distanceFromPlayer.Min();    
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