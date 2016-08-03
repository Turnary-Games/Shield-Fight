using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {

    public GameObject shieldParticlePrefab;

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player")
        {
            GameObject clone = Instantiate(shieldParticlePrefab, transform.position, transform.rotation) as GameObject;
            Destroy(clone, 2);
        }
    }
}
