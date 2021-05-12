using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapColliders : MonoBehaviour
{
    void OnCollisionEnter(Collision col) {
        if (col.collider.tag == "Goal") {
            Physics.IgnoreCollision(GetComponent<Collider>(), col.collider);
        }
    }
}
