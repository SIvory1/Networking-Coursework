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
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        enemyState = EnemyState.patrol;

        isAgro = true;

        priorX = transform.position.x;
    }

    // sync when they join
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.patrol:
                {      
                    MoveEnemy();
                    EnemyPosUpdateClientRpc(enemyPos, isAgro);
                    SpriteFlipClientRpc(currentX, priorX);
                }
                break;
            case EnemyState.seek:
                {
                    FindClosestPlayerClientRpc();
                    if (isAgro)
                    {
                        SeekClientRpc(enemyPos, closetPlayer);
                        SpriteFlipClientRpc(currentX, priorX);
                    }             
                }
                break;
        }
    }

    // lock to x and flip to look
    [ClientRpc]
    public void SeekClientRpc(Vector3 _enemyPos, Vector3 _closetPlayer)
    {
        Vector3 updatedEniPos = new(_enemyPos.x, _enemyPos.y, 0);
        Vector3 updatedPlayerPos = new(_closetPlayer.x, _enemyPos.y, 0);

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

        if (IsServer)
        {
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
    }

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
    private void EnemyPosUpdateClientRpc(Vector3 _enemyPos, bool _isAgro)
    {
        if (_isAgro)
        {
            transform.position = _enemyPos;    
        }
    }

    private void MoveEnemy()
    {
        if (!isAgro) return;
        transform.position = new Vector3(Mathf.PingPong(Time.time * speed, distance), transform.position.y, 0);
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
                    _player = GameObject.FindGameObjectsWithTag("Player");
                    print(_player.Length);
                    distanceFromPlayer = new float[_player.Length];
                    enemyState = EnemyState.seek;
                    break;
               case (5):
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
        GameManager.instance.audioManager.PlayDeathServerRPC();
        GameManager.instance.CheckEnemyCount();
        Destroy(gameObject);
    }
}