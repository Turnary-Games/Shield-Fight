using UnityEngine;
using System.Collections;
using ResourceDatabase;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

	/*
		CONSTANTS
	*/
	public const int NUM_OF_PLAYERS = 4;
	
	public string INPUT_PREFIX { get {						return "P" + player + " ";												} }
	public string INPUT_HORIZTONAL { get {					return INPUT_PREFIX + "Horizontal";										} }
	public string INPUT_VERTICAL { get {					return INPUT_PREFIX + "Vertical";										} }
	public string INPUT_FIRE { get {						return INPUT_PREFIX + "Fire";											} }
	public string INPUT_PUSH { get {						return INPUT_PREFIX + "Push";											} }

	public int LAYER_PLAYER { get {							return LayerMask.NameToLayer("Player " + player);						} }
	public int LAYER_HELD_SHIELD { get {					return LayerMask.NameToLayer("Shield " + player);						} }
	public int LAYER_THROWN_SHIELD { get {					return LayerMask.NameToLayer("Player " + player);						} }

	public bool initialized { get {							return resources != null && resources.PLAYER_ID == player;				} }

	[System.NonSerialized]
	public PlayerResource resources;

	/*
		PROPERTIES
	*/
	[Range(1, NUM_OF_PLAYERS)]
	public int player = 1;
	public int health = 20;
	[Header("Movement")]
	public float speed = 1200;
	public LayerMask raycastLayer = 1;
	[Header("Shield")]
	public GameObject shieldPrefab;
	public Transform shieldCenter;
	public float pickupRange = 2;
	[Header("Pushing")]
	public float pushCooldown = .5f;
	public float pushImpulseVsShield = 75f;
	public float pushImpulseVsPlayer = 200f;
	public float pushImpulseVsOther = 200f;
	[Header("Collision colliders collisions collide")]
	public Collider heldShieldCollider;
	public Collider playerCollider;
	public Collider pushTrigger;

	/*
		PRIVATE VARIABLES
	*/
	private Rigidbody body;
	private Shield shield;
	private bool armed { get { return shield == null; } }
	private bool inRange = true;
	private float pushTimestamp = -Mathf.Infinity;
	private ParticleSystem pushParticles;

	/*
		METHODS
	*/
	#if UNITY_EDITOR
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position.new_y(0), pickupRange);
	}
	#endif

	void Awake() {
		body = GetComponent<Rigidbody>();

		InitializePlayer();
	}
	
	void Update () {
		if (!initialized) return;

		#region Movement
		// Cant move while attracting
		if (shield == null || !shield.attracting) {
			// Real simple; read input, add force from input.
			Vector2 axis = new Vector2(Input.GetAxisRaw(INPUT_HORIZTONAL), Input.GetAxisRaw(INPUT_VERTICAL));
			axis.Scale(axis.normalized.Abs());

			Vector3 movement = axis.xzy(0) * speed * Time.deltaTime;

			body.AddForce(movement, ForceMode.VelocityChange);
		}
		#endregion

		#region Rotation
		body.angularVelocity = Vector3.zero;
		transform.eulerAngles = Vector3.zero;

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer)) {
			shieldCenter.transform.eulerAngles = new Vector3(0, (hit.point - shieldCenter.transform.position).zx().ToDegrees(), 0);
		}
		#endregion

		#region Shield shooting
		if (armed && Input.GetButtonDown(INPUT_FIRE)) {
			GameObject clone = Instantiate(shieldPrefab, shieldCenter.position, shieldCenter.rotation) as GameObject;

			// Add reference to /this/
			shield = clone.GetComponent<Shield>();
			shield.owner = this;

			// Add force
			shield.body = clone.GetComponent<Rigidbody>();
			shield.body.AddForce(shield.transform.forward * shield.speed, ForceMode.Impulse);

			// Set layer
			foreach (var t in shield.GetComponentsInChildren<Transform>())
				t.gameObject.layer = LAYER_THROWN_SHIELD;

			// Disable visual
			foreach (var ren in shieldCenter.GetComponentsInChildren<MeshRenderer>())
				ren.enabled = false;

			foreach (var ps in shieldCenter.GetComponentsInChildren<ParticleSystem>()) {
				if (ps.transform.IsChildOf(pushTrigger.transform)) continue;

				var em = ps.emission;
				em.enabled = false;
				ps.Clear();
			}

			// Disable protective shield on player
			heldShieldCollider.enabled = false;
		}
		#endregion

		#region Pickup shield
		if (!armed) {
			bool old = inRange;
			inRange = (shield.transform.position - transform.position).xz().magnitude <= pickupRange;
			if (inRange && !old) {
				PickupShield();
			}
		}
		#endregion

		#region Pushing
		// Can push if it's off cooldown + shield is not thrown
		bool canPush = Time.time - pushTimestamp > pushCooldown && armed;
		if (canPush && Input.GetButtonDown(INPUT_PUSH)) {
			StartCoroutine(PushCourotine());
		}
		#endregion
	}

	void OnTriggerEnter(Collider other) {
		// Compared to many, we ignore if someone triggers us
		if (other.isTrigger) return;

		// Only proceed if it has a rigidbody
		Rigidbody otherBody = other.attachedRigidbody;
		if (otherBody) {
			// Calculate which way to push 'em
			Vector3 delta = otherBody.transform.position - transform.position;
			delta.y = 0;

			if (otherBody.GetComponent<Player>())
				// Push away dat player
				otherBody.AddForce(delta.normalized * pushImpulseVsPlayer, ForceMode.Impulse);
			else if (otherBody.GetComponent<Shield>())
				// Be'gone, foul plate!
				otherBody.AddForce(delta.normalized * pushImpulseVsShield, ForceMode.Impulse);
			else
				// Get away from me unknown scrub!
				otherBody.AddForce(delta.normalized * pushImpulseVsOther, ForceMode.Impulse);
		}
	}

	IEnumerator PushCourotine() {
		pushTimestamp = Time.time;
		pushTrigger.enabled = true;
		pushParticles.Play();
		yield return new WaitForFixedUpdate();
		pushTrigger.enabled = false;
	}

	public void PickupShield() {
		if (!initialized) return;
		if (armed) return;

		// Move particlesystem away
		var ps = shield.GetComponentInChildren<ParticleSystem>();
		ps.transform.parent = null;
		var em = ps.emission;
		em.enabled = false;

		// Self destruct
		Destroy(ps.gameObject, ps.startLifetime);
		Destroy(shield.gameObject);

		// Enable visual
		foreach (var ren in shieldCenter.GetComponentsInChildren<MeshRenderer>())
			ren.enabled = true;

		foreach (var ps2 in shieldCenter.GetComponentsInChildren<ParticleSystem>()) {
			if (ps2.transform.IsChildOf(pushTrigger.transform)) continue;

			var em2 = ps2.emission;
			em2.enabled = true;
		}

		// Enable protective shield on player
		heldShieldCollider.enabled = true;

		shield = null;
		inRange = true;
	}

	#region <METHODS> Initialization and resetting
	public void InitializePlayer() {
		ResetPlayer();

		// Load resources
		if (resources == null || resources.PLAYER_ID != player) {
			resources = PlayerResource.FetchPlayerResources(player);
		}

		// Spawn in model
		resources.RESOURCE_CHARACTER_MODEL.Clone().transform.SetParent(transform, false);
		resources.RESOURCE_SHIELD_HELD_MODEL.Clone().transform.SetParent(shieldCenter, false);
		pushParticles = resources.RESOURCE_PUSH_PARTICLES.Clone().GetComponent<ParticleSystem>();
		pushParticles.transform.SetParent(pushTrigger.transform, false);

		// Change layer
		foreach (var t in GetComponentsInChildren<Transform>()) {
			if (t.IsChildOf(shieldCenter))
				t.gameObject.layer = LAYER_HELD_SHIELD;
			else
				t.gameObject.layer = LAYER_PLAYER;
		}
	}

	public void ResetPlayer() {
		// Remove model if any
		foreach (var t in GetComponentsInChildren<Transform>()) {
			if (t != null && IsItExtra(t)) {
				DestroyImmediate(t.gameObject);
			}
		}

		// Change layer
		foreach (var t in GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = 0;
		}

		resources = null;
	}

	public bool IsReset() {
		// Check for inregularities
		foreach (var t in GetComponentsInChildren<Transform>()) {
			if (IsItExtra(t)) {
				return false;
			}
		}

		return true;
	}

	public bool IsItExtra(Transform other) {
		if (other == transform) return false;
		if (other == shieldCenter) return false;
		if (playerCollider && other == playerCollider.transform) return false;
		if (heldShieldCollider && other == heldShieldCollider.transform) return false;
		if (pushTrigger && other == pushTrigger.transform) return false;

		return true;
	}
	#endregion
}
