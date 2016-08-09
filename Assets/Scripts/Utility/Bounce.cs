using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {

    private Rigidbody body;

    public float speed = 500;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = Random.Range(0, 360).FromDegrees().xzy(0);
    }

    void FixedUpdate()
    {
            body.velocity = body.velocity.normalized * speed / body.mass;
    }
/*
    void OnCollisionEnter(Collision col)
    {
        bounces++;

        var main = col.collider.GetMainObject();
        var player = main.GetComponent<Player>();

        if (player != null && player != owner)
        {

            // Check if we collided with the player collider, not the shield
            if (col.collider == player.playerCollider)
            {
                player.health--;
                owner.PickupShield();
            }

            //if (col.collider == player.heldShieldCollider) {
            //	// ...
            //}
        }
    }*/
}
