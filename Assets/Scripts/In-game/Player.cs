using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using ResourceDatabase;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour {

	[HideInInspector]
	[SyncVar]
	public int HOST_ID;
	Player HOST;
	
	/*
		CONSTANTS
	*/
	public const int NUM_OF_PLAYERS = 4;
	
	public string INPUT_PREFIX { get {						return "P" + input + " ";												} }
	public string INPUT_MOVE_HORIZTONAL { get {				return INPUT_PREFIX + "Move Horizontal";								} }
	public string INPUT_MOVE_VERTICAL { get {				return INPUT_PREFIX + "Move Vertical";									} }
	public string INPUT_LOOK_HORIZTONAL { get {				return INPUT_PREFIX + "Look Horizontal";								} }
	public string INPUT_LOOK_VERTICAL { get {				return INPUT_PREFIX + "Look Vertical";									} }
	public string INPUT_FIRE { get {						return INPUT_PREFIX + "Fire";											} }
	public string INPUT_PUSH { get {						return INPUT_PREFIX + "Push";											} }

	public int LAYER_PLAYER { get {							return LayerMask.NameToLayer("Player " + team);							} }
	public int LAYER_HELD_SHIELD { get {					return LayerMask.NameToLayer("Shield " + team);							} }
	public int LAYER_THROWN_SHIELD { get {					return LayerMask.NameToLayer("Player " + team);							} }

	public bool initialized { get {							return resources != null && resources.PLAYER_ID == player;				} }

	[System.NonSerialized]
	public PlayerResource resources;

	/*
		PROPERTIES
	*/
	[Range(1, NUM_OF_PLAYERS)]
	[SyncVar]
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
	public Collider pushTrigger;

	[System.NonSerialized]
	public int input = 1;
	[HideInInspector]
	[SyncVar]
	public int team = 0;
	[HideInInspector]
	[SyncVar]
	public bool dead = true;
	//[HideInInspector]
	[SyncVar]
	public bool attracting = false;

	private static bool everyoneLoaded = false;
	public static bool[] playersLoaded;

	/*
		PRIVATE VARIABLES
	*/
	[System.NonSerialized]
	public Rigidbody body;
	private bool armed { get { return heldShieldCollider.enabled; } }
	//private float pushTimestamp = -Mathf.Infinity;
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

	public override void OnStartClient() {
		InitializePlayer();

		//if (isLocalPlayer)
			StartCoroutine(WaitTilEveryoneLoaded());
	}

	public override void OnStartServer() {
		
		// In case of standalone server
		if (!isClient)
			InitializePlayer();
	}

	IEnumerator WaitTilEveryoneLoaded() {
		HOST = GetFromPlayerID(HOST_ID);
		do {
			HOST.CmdPlayerLoaded(player);
			yield return new WaitForSeconds(.5f);
		} while (!everyoneLoaded);
		if (this == HOST) GameCountdown.instance.CmdStartCountdown();
	}

	void Awake() {
		body = GetComponent<Rigidbody>();
		transform.position = transform.position.new_y(-10);
	}
	
	void Update () {
		if (!initialized) return;
		if (!isLocalPlayer) return;

		#region Rotation
		body.angularVelocity = Vector3.zero;
		transform.eulerAngles = Vector3.zero;

		if (input == 1) {
			// Rotate using mouse
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer)) {
				shieldCenter.transform.eulerAngles = new Vector3(0, (hit.point - shieldCenter.transform.position).zx().ToDegrees(), 0);
			}
		} else {
			// Rotate using input
			Vector2 lookAxis = new Vector2(Input.GetAxis(INPUT_LOOK_HORIZTONAL), Input.GetAxis(INPUT_LOOK_VERTICAL));
			shieldCenter.transform.eulerAngles = new Vector3(0, lookAxis.ToDegrees(), 0);
		}
		#endregion

		if (dead) return;

		if (isClient) {
			bool old_attracting = attracting;
			attracting = Input.GetButton(INPUT_FIRE) || Input.GetButton(INPUT_PUSH);
			if (attracting != old_attracting) {
				CmdSetAttracting(attracting);
			}
		}

		#region Movement
		// Cant move while attracting
		//if (shield == null || !shield.attracting) {
		// Real simple; read input, add force from input.
		Vector2 moveAxis = new Vector2(Input.GetAxisRaw(INPUT_MOVE_HORIZTONAL), Input.GetAxisRaw(INPUT_MOVE_VERTICAL));
			moveAxis.Scale(moveAxis.normalized.Abs());

			Vector3 movement = moveAxis.xzy(0) * speed * Time.deltaTime;

			body.AddForce(movement, ForceMode.VelocityChange);
		//}
		#endregion

		#region Shield shooting
		if (armed && Input.GetButtonDown(INPUT_FIRE)) {
			heldShieldCollider.enabled = false;
			CmdShootShield();
		}
		#endregion
	}
	

	#region Server-side only methods
	[Command]
	void CmdSetAttracting(bool state) {
		attracting = state;
	}

	[Command]
	public void CmdPickupShield() {
		heldShieldCollider.enabled = true;
		RpcPickupShield();
	}

	[Command]
	public void CmdShootShield() {
		if (dead) return;

		GameObject clone = Instantiate(shieldPrefab, shieldCenter.position, shieldCenter.rotation) as GameObject;

		// Add reference to /this/
		var shield = clone.GetComponent<Shield>();
		shield.owner_id = player;
		shield.forward = shieldCenter.forward;

		// Set layer
		foreach (var t in shield.GetComponentsInChildren<Transform>())
			t.gameObject.layer = LAYER_THROWN_SHIELD;

		//NetworkServer.SpawnWithClientAuthority(clone, gameObject);
		NetworkServer.Spawn(clone);

		// Disable protective shield on player
		heldShieldCollider.enabled = false;
		RpcShootShield();
	}

	[Command]
	public void CmdPartycles(Vector3 position, Vector3 euler) {
		RpcPartycles(position, euler);
	}

	[Command]
	public void CmdKill() {
		dead = true;
		RpcDie();
		GameMatchPoint.instance.CmdOnPlayerDied(player);
	}

	[Command]
	public void CmdPlayerLoaded(int playerID) {
		if (everyoneLoaded) return;

		playersLoaded[playerID - 1] = true;

		for (int i = 0; i < playersLoaded.Length; i++)
			if (!playersLoaded[i]) return;
			
		RpcEveryoneLoaded();
		GameStats.instance.RpcOnStartClient();
	}
	#endregion

	#region Client-side only methods
	[ClientRpc]
	public void RpcJumpToSpawnpoint() {
		transform.position = SpawnPoints.GetSpawningPosition(player, FindObjectsOfType<Player>().Length).new_y(1);
		dead = true;
		body.velocity = Vector3.zero;
	}

	[ClientRpc]
	public void RpcDie() {
		dead = true;
		transform.position = new Vector3(0, -10, 0);
		body.velocity = Vector3.zero;
	}

	[ClientRpc]
	public void RpcStartGame() {
		dead = false;
	}

	[ClientRpc]
	public void RpcPickupShield() {
		heldShieldCollider.enabled = true;

		// Enable visual
		foreach (var ren in shieldCenter.GetComponentsInChildren<MeshRenderer>())
			ren.enabled = true;

		foreach (var ps2 in shieldCenter.GetComponentsInChildren<ParticleSystem>()) {
			if (ps2.transform.IsChildOf(pushTrigger.transform)) continue;

			var em2 = ps2.emission;
			em2.enabled = true;
		}
	}

	[ClientRpc]
	public void RpcShootShield() {
		heldShieldCollider.enabled = false;

		// Disable visual
		foreach (var ren in shieldCenter.GetComponentsInChildren<MeshRenderer>())
			ren.enabled = false;

		foreach (var ps in shieldCenter.GetComponentsInChildren<ParticleSystem>()) {
			if (ps.transform.IsChildOf(pushTrigger.transform)) continue;

			var em = ps.emission;
			em.enabled = false;
			ps.Clear();
		}
	}

	[ClientRpc]
	public void RpcPartycles(Vector3 position, Vector3 euler) {
		GameObject clone = GameObject.Instantiate(resources.RESOURCE_SHIELD_HIT_PARTICLES, position, Quaternion.Euler(euler)) as GameObject;
		Destroy(clone, 2);
	}

	[ClientRpc]
	public void RpcEveryoneLoaded() {
		everyoneLoaded = true;
	}
	#endregion

	#region <METHODS> Initialization and resetting
	public void InitializePlayer() {
		ResetPlayer();

		// Load resources
		if (resources == null || resources.PLAYER_ID != player) {
			resources = PlayerResource.FetchPlayerResources(player);
		}

		if (isClient) {
			// Spawn in model
			resources.RESOURCE_CHARACTER_MODEL.Clone().transform.SetParent(transform, false);
			resources.RESOURCE_SHIELD_HELD_MODEL.Clone().transform.SetParent(shieldCenter, false);
			pushParticles = resources.RESOURCE_PUSH_PARTICLES.Clone().GetComponent<ParticleSystem>();
			pushParticles.transform.SetParent(pushTrigger.transform, false);
		}

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

	public static Player GetFromPlayerID(int id) {
		return new List<Player>(FindObjectsOfType<Player>()).Find(p => p.player == id);
	}
}
