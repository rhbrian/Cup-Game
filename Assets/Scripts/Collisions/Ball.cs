using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ball : NetworkBehaviour
{
    void OnCollisionEnter(Collision col) {
        if (col.collider.tag == "Goal") {
            Debug.Log("Goal!");
        }
    }
}
