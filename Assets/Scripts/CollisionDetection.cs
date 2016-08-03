using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player Hit ");
        }
    }
}
