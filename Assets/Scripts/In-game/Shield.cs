using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Shield : NetworkBehaviour {

	[HideInInspector]
	[SyncVar]
	public int owner_id;
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
	[SyncVar]
	public bool doneBouncing = false;
	private Player owner;
	[HideInInspector]
	[SyncVar]
	public Vector3 forward;

	void Awake() {
		body = GetComponent<Rigidbody>();
	}

	public override void OnStartClient() {

		owner = Player.GetFromPlayerID(owner_id);

		// Set layer
		foreach (var t in GetComponentsInChildren<Transform>())
			t.gameObject.layer = owner.LAYER_THROWN_SHIELD;

		// Spawn in model
		owner.resources.RESOURCE_SHIELD_THROWN_MODEL.Clone().transform.SetParent(transform, false);
	}

	public override void OnStartServer() {
		start = Time.time;
		body.velocity = forward * speed / body.mass;
		owner = owner ?? Player.GetFromPlayerID(owner_id);

		if (!isClient) {
			// Set layer
			foreach (var t in GetComponentsInChildren<Transform>())
				t.gameObject.layer = owner.LAYER_THROWN_SHIELD;
		}
	}
	
	void FixedUpdate() {
		#region Move towards player
		if (doneBouncing) {
			
			if (owner.attracting && !owner.dead) {
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
		if (!isServer) return;
		if (!doneBouncing && (bounces > maxNumberOfBounces || Time.time - start > maxTime))
			doneBouncing = true;

		// Set Y position depending on current speed
		transform.position = transform.position.new_y(Mathf.Lerp(minY, maxY, body.velocity.magnitude / (minSpeed / body.mass)));

		if (!owner.dead) {
			if (doneBouncing && (owner.transform.position - transform.position).new_y(0).magnitude <= owner.pickupRange) {
				NetworkServer.Destroy(gameObject);
				owner.CmdPickupShield();
			}
		}
	}
	
	void OnCollisionEnter(Collision col) {
		if (!isServer) return;
		bounces++;

		var main = col.collider.GetMainObject();
		var player = main.GetComponent<Player>();

		if (player != null && player.player != owner_id) {
			// Check if we collided with the player collider, not the shield
			if (col.collider == player.playerCollider) {

				player.CmdKill();
				owner.CmdPickupShield();

				NetworkServer.Destroy(gameObject);

				// Collided with player, TIME FOR PARTYCLES
				owner.CmdPartycles(col.contacts[0].point, new Vector3(0, body.velocity.zx().ToDegrees(), 0));
			}

			// Check if we collided with the held shield, not the player body
			//if (col.collider == player.heldShieldCollider) {
			//	// ...
			//}
		}
	}

	#region Server-side only methods
	#endregion

	#region Client-side only methods
	#endregion
}
