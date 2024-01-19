using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{

    int colCount;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            colCount++;
            if (colCount == 2)
            {
                Destroy(gameObject);
            }
        }
    }
}
