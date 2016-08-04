using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

	string INPUT_HORIZTONAL { get { return Globals.Player.INPUT_PREFIX + player + Globals.Player.INPUT_HORIZONTAL_SUFFIX; } }
	string INPUT_VERTICAL { get { return Globals.Player.INPUT_PREFIX + player + Globals.Player.INPUT_VERTICAL_SUFFIX; } }
	string INPUT_FIRE { get { return Globals.Player.INPUT_PREFIX + player + Globals.Player.INPUT_FIRE_SUFFIX; } }
	string INPUT_PUSH { get { return Globals.Player.INPUT_PREFIX + player + Globals.Player.INPUT_PUSH_SUFFIX; } }
	Color COLOR { get { return Globals.Player.COLORS[player - 1]; } }

	[Range(1, Globals.Player.NUM_OF_PLAYERS)]
	public int player = 1;
	public int health = 20;
	[Header("Movement")]
	public float speed = 1200;
	[Header("Shield")]
	public GameObject shieldPrefab;
	public Transform shieldCenter;
	public float pickupRange = 2;

	private Rigidbody body;
	private Shield shield;
	private bool armed { get { return shield == null; } }
	private bool inRange = true;

	#if UNITY_EDITOR
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position.new_y(0), pickupRange);
	}
	#endif

	void Awake() {
		body = GetComponent<Rigidbody>();
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
		if (Physics.Raycast(ray, out hit)) {
			shieldCenter.transform.eulerAngles = new Vector3(0, (hit.point - shieldCenter.transform.position).zx().ToDegrees(), 0);
		}	
		#endregion

		#region Shield shooting
		if (armed && Input.GetButtonDown(INPUT_FIRE)) {
			GameObject clone = Instantiate(shieldPrefab, shieldCenter.position, shieldCenter.rotation) as GameObject;
			
			shield = clone.GetComponent<Shield>();
			shield.owner = this;

			shield.body = clone.GetComponent<Rigidbody>();
			shield.body.AddForce(shield.transform.forward * shield.speed, ForceMode.Impulse);

			foreach (var t in shield.GetComponentsInChildren<Transform>()) {
				t.gameObject.layer = gameObject.layer;
			}

			// Disable visual
			foreach (var ren in shieldCenter.GetComponentsInChildren<MeshRenderer>())
				ren.enabled = false;

			foreach (var ps in shieldCenter.GetComponentsInChildren<ParticleSystem>()) {
				var em = ps.emission;
				em.enabled = false;
				ps.Clear();
			}
		}
		#endregion

		#region Pickup shield
		if (!armed) {
			bool old = inRange;
			inRange = (shield.transform.position - transform.position).xz().magnitude <= pickupRange;
			if (inRange && !old) {
				var ps = shield.GetComponentInChildren<ParticleSystem>();
				ps.transform.parent = null;
				var em = ps.emission;
				em.enabled = false;
				Destroy(ps.gameObject, ps.startLifetime);
				
				Destroy(shield.gameObject);

				// Enable visual
				foreach (var ren in shieldCenter.GetComponentsInChildren<MeshRenderer>())
					ren.enabled = true;

				foreach (var ps2 in shieldCenter.GetComponentsInChildren<ParticleSystem>()) {
					var em2 = ps2.emission;
					em2.enabled = true;
				}
			}
		}
		#endregion
	}


}
