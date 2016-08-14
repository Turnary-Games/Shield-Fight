using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

	[System.NonSerialized]
	public Player owner;
	[System.NonSerialized]
	public Rigidbody body;

	[Header("Movement")]
	public float speed = 50;
	public float attractSpeed = 35;
	[Header("Limiting factors")]
	public int maxNumberOfBounces = 8;
	public float maxTime = 5;
	public float dragAtLastBounce = 2;
	[Header("Floating-Y-pos")]
	public float minSpeed = 10;
	public float minY = 0;
	public float maxY = 1;

	[Header("Partycles")]
	public GameObject particlePrefab;

	private int bounces;
	private float start;
	private bool doneBouncing = false;

	[System.NonSerialized]
	public bool attracting = false;

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
		#region Move towards player
		if (doneBouncing) {
			attracting = Input.GetButton(owner.INPUT_FIRE) || Input.GetButton(owner.INPUT_PUSH);
			if (attracting) {
				// Calculate drag
				body.drag = Mathf.MoveTowards(body.drag, 0, Time.deltaTime * dragAtLastBounce);

				// Calculate vector, ignore Y diff
				Vector3 delta = owner.transform.position - transform.position;
				delta.y = 0;

				// Add as force
				float multiplier = 1 - body.drag / dragAtLastBounce;
				float topSpeed = attractSpeed / body.mass;
				Vector3 force = delta.normalized * topSpeed * Time.fixedDeltaTime;
				body.velocity = Vector3.ClampMagnitude(body.velocity + force, topSpeed * multiplier);
			} else {
				// Calculate drag
				body.drag = Mathf.MoveTowards(body.drag, dragAtLastBounce, Time.deltaTime * dragAtLastBounce);
			}
		}
		#endregion

		#region Keep constant speed
		// Depending on how long shield has flown
		if (!doneBouncing)
			// Keep a constant speed
			body.velocity = body.velocity.normalized * speed / body.mass;
		#endregion
	}

	void Update() {
		if (!doneBouncing && bounces < maxNumberOfBounces && Time.time - start < maxTime)
			doneBouncing = true;

		// Set Y position depending on current speed
		transform.position = transform.position.new_y(Mathf.Lerp(minY, maxY, body.velocity.magnitude / (minSpeed / body.mass)));
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

				// Collided with player, TIME FOR PARTYCLES
				GameObject clone = Instantiate(particlePrefab, transform.position, Quaternion.Euler(0, body.velocity.zx().ToDegrees(), 0)) as GameObject;
				Destroy(clone, 2);
			}

			// Check if we collided with the held shield, not the player body
			//if (col.collider == player.heldShieldCollider) {
			//	// ...
			//}
		}
	}
	
}
