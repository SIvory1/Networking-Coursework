using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{

    [Header("Player Inputs")]
    private float playerSpeed;
    [SerializeField] private float playerBaseSpeed;
    private float moveX;
    private float moveY;
 
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    private int jumpCounter;
    [SerializeField] private int jumpCounterMax;

    [Header("Dash")]
    [SerializeField] private float dashForce;
    private int dashCounter;
    [SerializeField] private int dashCounterMax;

    [Header("Bullet")]
    [SerializeField] private Transform bulletPrefab;
    Transform bulletSpawnTransform;
    [SerializeField] Transform spawnPoint;

    [Header("Misc")]
    private Animator playerAnimator;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    bool iFrame;

    public override void OnNetworkSpawn()
    {
        //this section will be triggered when a player enters / spawned into the game
        playerAnimator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
   
        jumpCounter = jumpCounterMax;
        dashCounter = dashCounterMax;
        playerSpeed = playerBaseSpeed;
    }

    private void Update()
    {
        if (!IsOwner) return;
        MovePlayer();
        Shoot();
        Dash();
        Jump();
        FlipPlayerServerRPC(moveX);
    }

    void MovePlayer()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (moveX < -0 || moveX > 0)
            playerAnimator.SetBool("isSprint", true);
        else 
            playerAnimator.SetBool("isSprint", false);

        rb.velocity = new Vector2(moveX * playerSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
    }

    [ClientRpc] void FlipPlayerClientRPC(float _moveX)
    {
        if (_moveX >= 0)
        {
            sprite.flipX = false;
        }
        else 
        {
            sprite.flipX = true;
        }
    }
    [ServerRpc] void FlipPlayerServerRPC(float _moveX)
    {
        FlipPlayerClientRPC(_moveX);
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCounter != 0)
        {
            //checks direction
            if (moveX > 0.1)
            {
                rb.AddForce(Vector2.right * dashForce);
            }
            else if (moveX < -0.1)
            {
                rb.AddForce(Vector2.left * dashForce);
            }
            StartCoroutine(DashCooldown());
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCounter != 0 || moveY > 0.01 && jumpCounter != 0)
        {
            playerAnimator.SetBool("isJump", true);
            rb.AddForce(Vector2.up * jumpForce);
            jumpCounter--;
        }
    }

    IEnumerator DashCooldown()
    {
        dashCounter--;
        yield return new WaitForSeconds(1);
        dashCounter = dashCounterMax;
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InstantiateBulletServerRpc(spawnPoint.position.x, spawnPoint.position.y, 0, spawnPoint.rotation);
            GameManager.instance.audioManager.PlayZapServerRPC();
        }
    }

    [ServerRpc]
    private void InstantiateBulletServerRpc(float x, float y, float z, Quaternion rot)
    {
        bulletSpawnTransform = Instantiate(bulletPrefab, new Vector3(x, y, z), rot);
        bulletSpawnTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        if (!IsOwner) return;

        if (target.gameObject.CompareTag("Ground") || target.gameObject.CompareTag("Enemy")|| target.gameObject.CompareTag("Player"))
        {
            jumpCounter = jumpCounterMax;
            playerAnimator.SetBool("isJump", false);
        }
        if (target.gameObject.CompareTag("Enemy"))
        {
            // do something
            CollideWithEnemyServerRpc(iFrame);
        }
    }

    [ClientRpc]
    void CollideWithEnemyClientRpc(bool _iFrame)
    {
        if (_iFrame) return; 
        rb.AddForce(Vector2.up * 250);
        GetComponentInChildren<SpriteRenderer>().color = Color.red;
        if (IsOwner) { GetComponent<PlayerValues>().TakeDamage(1); }
        StartCoroutine(ResetIframes());
        iFrame = true;
    }

   
    [ServerRpc]
    void CollideWithEnemyServerRpc(bool _iFrame)
    {
        CollideWithEnemyClientRpc(_iFrame);
    }

    IEnumerator ResetIframes()
    {
        yield return new WaitForSeconds(1f);
        iFrame = false;
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

}