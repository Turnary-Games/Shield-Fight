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
	[Header("Floating-pos")]
	public float minY = 0;
	public float maxY = 1;

	[Header("Partycles")]
	public GameObject particlePrefab;

	private int bounces;
	private float start;

	void Start() {
		if (!owner || !body) {
			Destroy(gameObject);
			return;
		}
		
		start = Time.time;

		// Spawn in model
		owner.resources.RESOURCE_SHIELD_THROWN_MODEL.Clone().transform.SetParent(transform, false);
	}

	void FixedUpdate() {
		if (body && bounces < maxNumberOfBounces && Time.time - start < maxTime)
			body.velocity = body.velocity.normalized * speed / body.mass;
		else if (body)
			body.drag = dragAtLastBounce;
	}

	void Update() {
		transform.position = transform.position.new_y(Mathf.Lerp(minY, maxY, body.velocity.magnitude / (speed / body.mass)));
	}

	void OnCollisionEnter(Collision col) {
		bounces++;

		var main = col.collider.GetMainObject();
		var player = main.GetComponent<Player>();

		if (player != null && player != owner) {

			// Collided with player, TIME FOR PARTYCLES
			GameObject clone = Instantiate(particlePrefab, transform.position, transform.rotation) as GameObject;
			Destroy(clone, 2);

			// Check if we collided with the player collider, not the shield
			if (col.collider == player.playerCollider) {
				player.health--;
				owner.PickupShield();
			}

			// Check if we collided with the held shield, not the player body
			//if (col.collider == player.heldShieldCollider) {
			//	// ...
			//}
		}
	}
	
}
