using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class testing : MonoBehaviour {

	public Vector2 anchorMin;
	public Vector2 anchorMax;
	public Vector2 offsetMin;
	public Vector2 offsetMax;
	public bool run = false;

	private RectTransform rect;
	
	void Awake () {
		rect = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (run) { run = false;

			rect.anchorMin = anchorMin;
			rect.anchorMax = anchorMax;
			rect.offsetMin = offsetMin;
			rect.offsetMax = offsetMax;
		}
	}
}
