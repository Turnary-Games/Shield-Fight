using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AutoCameraZoom : MonoBehaviour {

	public float minWidth = 5;
	public float minHeight = 5;
	public bool runInEditor = false;

	private Camera cam;

	void Awake() {
		cam = GetComponent<Camera>();
	}
	
	void LateUpdate () {
		if (Application.isPlaying || runInEditor) {

			// aspect = width / height
			// width = aspect * height
			// height = width / aspect

			var minSize = minHeight * .5f;
			var variedSize = (minWidth / cam.aspect) * .5f; // Height based off the current aspect ratio
			cam.orthographicSize = Mathf.Max(minSize, variedSize);
		}
	}
}
