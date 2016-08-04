using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

	[System.NonSerialized]
	public Player owner;
	[System.NonSerialized]
	public Rigidbody body;

	public float speed = 500;
	public int maxNumberOfBounces = 8;
	public float maxTime = 5;
	public float dragAtLastBounce = 2;

	private int bounces;
	private float start;

	void Start() {
		start = Time.time;

		if (owner) {
			foreach (var ren in GetComponentsInChildren<MeshRenderer>()) {
				ren.sharedMaterial = owner.RESOURCE_SHIELD_MATERIAL;
			}
		} else
			Debug.LogError("This shield lives without it's owner!");
	}

	void FixedUpdate() {
		if (body && bounces < maxNumberOfBounces && Time.time - start < maxTime)
			body.velocity = body.velocity.normalized * speed / body.mass;
		else if (body)
			body.drag = dragAtLastBounce;
	}

	void OnCollisionEnter(Collision col) {
		bounces++;

		var main = col.collider.GetMainObject();
		var player = main.GetComponent<Player>();

		if (player != null && player != owner) { 
			
			// Check if we collided with the player collider, not the shield
			if (col.collider == player.playerCollider) {
				player.health--;
				owner.PickupShield();
			}

			//if (col.collider == player.heldShieldCollider) {
			//	// ...
			//}
		}
	}
	
}
