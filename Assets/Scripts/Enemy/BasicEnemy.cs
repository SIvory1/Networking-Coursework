using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    int enemyHealth;
    [SerializeField] private float distance;
    [SerializeField] private float speed;

    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);

        //rb.velocity = new Vector2(transform.position.x, transform.position.y) * speed;
    }

    int flipCounter;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            enemyHealth++;
            if (enemyHealth == 2)
            {
                Destroy(gameObject);
            }
        }
         
       if (!col.gameObject.CompareTag("Bullet") || !col.gameObject.CompareTag("Ground"))
       {
            if (flipCounter == 0)
            {     
                transform.Rotate(180f, 180f, 180f);
                flipCounter++;
            }  
       }
    }
}