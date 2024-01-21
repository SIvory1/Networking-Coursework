using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField] private bool verticalPlatform;
    [SerializeField] private float distance;
    [SerializeField] private float speed;

    void Update()
    {
        if (!verticalPlatform)
         transform.position = new Vector3(distance * Mathf.Cos(Time.time * speed), transform.position.y, transform.position.z);
          else
            transform.position = new Vector3(transform.position.x, distance * Mathf.Cos(Time.time * speed), transform.position.z);
    }
}
