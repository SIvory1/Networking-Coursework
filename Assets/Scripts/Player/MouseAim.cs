using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseAim : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
      //  if (!IsOwner) return;
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        var dir = Input.mousePosition - pos;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}