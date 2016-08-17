using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class DisplayVersion : SingletonBase<DisplayVersion> {
	private string downloadPage;
	private string current;
	private string latest;

	private GUIStyle currentStyle;
	private GUIStyle latestStyle;

	protected override void Awake() {
		if (instance != null) Destroy(gameObject);
		instance = this;
		transform.parent = null;
		DontDestroyOnLoad(transform);
	}

	void OnGUI() {
		GUI.Label(new Rect(Screen.width - 230, 10, 200, 25), Application.productName + " " + current, currentStyle);
		if (!string.IsNullOrEmpty(downloadPage)) {
			var content = new GUIContent("New version available! (" + latest + ")");
			float minWidth, maxWidth, height;
			latestStyle.CalcMinMaxWidth(content, out maxWidth, out minWidth);
			height = latestStyle.CalcHeight(content, maxWidth);

			if (GUI.Button(new Rect(Screen.width - 30 - maxWidth, 35, maxWidth, height), content, latestStyle)) {
				Application.OpenURL(downloadPage);
			}
		}
	}

	IEnumerator Start() {
		if (instance == this) {

			current = FormatVersion(Globals.Version.CURRENT);

			currentStyle = new GUIStyle();
			currentStyle.normal.textColor =
			currentStyle.onNormal.textColor = Color.gray;
			currentStyle.alignment = TextAnchor.MiddleRight;

			latestStyle = new GUIStyle();
			latestStyle.normal.textColor =
			latestStyle.onNormal.textColor = Color.gray;
			latestStyle.hover.textColor =
			latestStyle.onHover.textColor = Color.white;
			latestStyle.alignment = TextAnchor.MiddleRight;

			var tex = new Texture2D(1, 1);
			tex.SetPixel(1, 1, Color.clear);
			tex.Apply();

			latestStyle.normal.background =
			latestStyle.onNormal.background =
			latestStyle.hover.background =
			latestStyle.onHover.background = tex;

			// Fetch latest version
			using (WWW www = new WWW(Globals.Version.LATEST_URL)) {
				// Wait for HTTP request
				yield return www;

				// Error checking
				if (string.IsNullOrEmpty(www.text)) goto Failed;  // Got data
				if (!string.IsNullOrEmpty(www.error)) goto Failed; // Some error

				// Parse JSON data
				ReleaseData data = (ReleaseData)JsonUtility.FromJson(www.text, typeof(ReleaseData));
				if (data == null) goto Failed; // Failed to parse data

				// Trim away everything but numbers and dots
				string tag = Regex.Replace(data.tag_name, "[^0-9.^\\.]", "");
				// Parse into version object
				Version latest = new Version(tag);

				// Check if latest is newer than current
				if (Globals.Version.CURRENT.CompareTo(latest) < 0) {
					downloadPage = data.html_url;
					this.latest = FormatVersion(latest);
				}
				goto End;

			Failed:
				print("FAILED TO CHECK LATEST VERSION!");

			End:;
			}
		}
	}

	public static string FormatVersion(Version version) {
		return string.Format("v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
	}

	[Serializable]
	public class ReleaseData {
		public string url;
		public string assets_url;
		public string upload_url;
		public string html_url;
		public int id;
		public string tag_name;
		public string target_commitish;
		public string name;
		public bool draft;
		public bool prerelease;
		public string created_at;
		public string published_at;
	}
}
