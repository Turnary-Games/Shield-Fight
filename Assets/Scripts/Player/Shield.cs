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
	}

	void FixedUpdate() {
		if (body && bounces < maxNumberOfBounces && Time.time - start < maxTime)
			body.velocity = body.velocity.normalized * speed / body.mass;
		else if (body)
			body.drag = dragAtLastBounce;
	}

	void OnCollisionEnter() {
		bounces++;
	}
	
}
