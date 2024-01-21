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

    public GameObject[] _player;
    public float[] distanceFromPlayer;
    Vector3 closetPlayer;

    Vector2 desiredVelocity;
    Vector3 steeringVelocity;

    bool isAgro;

    float currentX;
    float priorX;

    private void Start()
    {
        StartCoroutine(FindPlayers());
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        enemyState = EnemyState.patrol;

        isAgro = true;

        priorX = transform.position.x;
    }

    // cant press too quick or gamebreaks
    IEnumerator FindPlayers()
    {
        yield return new WaitForSeconds(5f);
        _player = GameObject.FindGameObjectsWithTag("Player");
        print(_player.Length);
        distanceFromPlayer = new float[_player.Length];
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
                    EnemyPosUpdateClientRPC(enemyPos, isAgro);
                    MoveEnemy();
                    SpriteFlipClientRpc(currentX, priorX);
                }
                break;
            case EnemyState.seek:
                {
                    FindClosestPlayerClientRpc();
                    SeekClientRpc(enemyPos, closetPlayer);
                    SpriteFlipClientRpc(currentX, priorX);
                }
                break;
        }
    }


    // lock to x and flip to look
    [ClientRpc]
    public void SeekClientRpc(Vector3 _enemyPos, Vector3 _closetPlayer)
    {
        Vector3 updatedEniPos = new(_enemyPos.x, 0, 0);
        Vector3 updatedPlayerPos = new(_closetPlayer.x, 0, 0);

        // Get the desired velocity for seek and limit to maxSpeed
        desiredVelocity = Vector3.Normalize(updatedPlayerPos - updatedEniPos) * speed;

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
                distanceFromPlayer[i] = (_player[i].transform.position - transform.position).magnitude;
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
    }

    //[ServerRpc]
    //private void EnemyPosUpdateServerRPC(Vector3 _enemyPos, bool _isAgro)
    //{
    //   EnemyPosUpdateClientRPC(_enemyPos, _isAgro);
    //}

    [ClientRpc]
    private void SpriteFlipClientRpc(float _currentX, float _priorX)
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

    [ClientRpc]
    private void EnemyPosUpdateClientRPC(Vector3 _enemyPos, bool _isAgro)
    {
        if (_isAgro)
        {
            transform.position = _enemyPos;    
        }
    }

    private void MoveEnemy()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time * speed, distance), -3.91f, 0);
        enemyPos = transform.position;     
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            enemyHealth++;
            switch (enemyHealth)
            {
                case (1):
                    enemyState = EnemyState.seek;
                    break;
               case (2):
                    isAgro = false;
                    animator.SetBool("TakenDMG", true);
                    StartCoroutine(DestroyEnemy());
                    break;
            }
        }      
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2);
        DestroyEnemyClientRpc();
    }

    [ClientRpc]
    void DestroyEnemyClientRpc()
    {
        Destroy(gameObject);
    }
}