using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

	/*
		CONSTANTS
	*/
	public const int NUM_OF_PLAYERS = 4;
	
	string INPUT_PREFIX { get {						return "P" + player + " ";												} }
	string INPUT_HORIZTONAL { get {					return INPUT_PREFIX + "Horizontal";										} }
	string INPUT_VERTICAL { get {					return INPUT_PREFIX + "Vertical";										} }
	string INPUT_FIRE { get {						return INPUT_PREFIX + "Fire";											} }
	string INPUT_PUSH { get {						return INPUT_PREFIX + "Push";											} }
	
	string PATH_BASE { get {						return "Player resources/Player " + player + " resources/";				} }
	string PATH_CHARACTER_MODEL { get {				return PATH_BASE + "Player model";										} }
	string PATH_SHIELD_HELD_MODEL { get {			return PATH_BASE + "Held shield model";									} }
	string PATH_SHIELD_THROWN_MODEL { get {			return PATH_BASE + "Thrown shield model";								} }

	[System.NonSerialized] public GameObject RESOURCE_CHARACTER_MODEL;
	[System.NonSerialized] public GameObject RESOURCE_SHIELD_HELD_MODEL;
	[System.NonSerialized] public GameObject RESOURCE_SHIELD_THROWN_MODEL;

	int LAYER_PLAYER { get {						return LayerMask.NameToLayer("Player " + player);						} }
	int LAYER_HELD_SHIELD { get {					return LayerMask.NameToLayer("Shield " + player);						} }
	int LAYER_THROWN_SHIELD { get {					return LayerMask.NameToLayer("Player " + player);						} }

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
	[Header("Collision colliders collisions collide")]
	public Collider heldShieldCollider;
	public Collider playerCollider;

	/*
		PRIVATE VARIABLES
	*/
	private Rigidbody body;
	private Shield shield;
	private bool armed { get { return shield == null; } }
	private bool inRange = true;

	
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

		// Load resources
		RESOURCE_CHARACTER_MODEL = Resources.Load(PATH_CHARACTER_MODEL) as GameObject;
		RESOURCE_SHIELD_HELD_MODEL = Resources.Load(PATH_SHIELD_HELD_MODEL) as GameObject;
		RESOURCE_SHIELD_THROWN_MODEL = Resources.Load(PATH_SHIELD_THROWN_MODEL) as GameObject;

		// Spawn in model
		GameObject clone;
		
		clone = Instantiate(RESOURCE_CHARACTER_MODEL) as GameObject;
		clone.transform.localPosition = RESOURCE_CHARACTER_MODEL.transform.localPosition;
		clone.transform.localRotation = RESOURCE_CHARACTER_MODEL.transform.localRotation;
		clone.transform.localScale = RESOURCE_CHARACTER_MODEL.transform.localScale;
		clone.transform.SetParent(transform, false);

		clone = Instantiate(RESOURCE_SHIELD_HELD_MODEL) as GameObject;
		clone.transform.localPosition = RESOURCE_SHIELD_HELD_MODEL.transform.localPosition;
		clone.transform.localRotation = RESOURCE_SHIELD_HELD_MODEL.transform.localRotation;
		clone.transform.localScale = RESOURCE_SHIELD_HELD_MODEL.transform.localScale;
		clone.transform.SetParent(shieldCenter, false);

		// Change layer
		foreach(var t in GetComponentsInChildren<Transform>()) {
			if (t.IsChildOf(shieldCenter))
				t.gameObject.layer = LAYER_HELD_SHIELD;
			else
				t.gameObject.layer = LAYER_PLAYER;
		}
	}
	
	void Update () {
		#region Movement
		Vector2 axis = new Vector2(Input.GetAxisRaw(INPUT_HORIZTONAL), Input.GetAxisRaw(INPUT_VERTICAL));
		axis.Scale(axis.normalized.Abs());

		Vector3 movement = axis.xzy(0) * speed * Time.deltaTime;

		body.AddForce(movement, ForceMode.VelocityChange);
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
	}

	public void PickupShield() {
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
			var em2 = ps2.emission;
			em2.enabled = true;
		}

		// Enable protective shield on player
		heldShieldCollider.enabled = true;

		shield = null;
		inRange = true;
	}

}
