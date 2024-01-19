using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileMovement : NetworkBehaviour
{

    [SerializeField] private float projectileSpeed;

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * projectileSpeed;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}