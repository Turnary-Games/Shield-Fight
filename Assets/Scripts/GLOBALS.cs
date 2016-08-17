using UnityEngine;
using System.Collections;
using System;

namespace Globals {

	public class Version {
		public static readonly System.Version CURRENT = new System.Version(0, 3, 1);
		public static readonly string LATEST_URL = "https://api.github.com/repos/Turnary-Games/Shield-Fight/releases/latest";
	}

	public class Input {
		public static readonly GUIContent[] DROPDOWN = new GUIContent[] {
			/* P1 */	new GUIContent("WASD + Mouse"),		
			/* P2 */	new GUIContent("Xbox 360 controller 1"),
			/* P3 */	new GUIContent("Xbox 360 controller 2"),
			/* P4 */	new GUIContent("Xbox 360 controller 3"),
			/* P5 */	new GUIContent("Xbox 360 controller 4"),
		};
	}

	public class Players {
		public static readonly Color[] COLORS = new Color[] {
			Color.yellow,
			Color.blue,
			Color.green,
			Color.red,
		};

		public static readonly GUIContent[] TEAMS = new GUIContent[] {
			new GUIContent("<No team>"),
			new GUIContent("Team 1"),
			new GUIContent("Team 2"),
			new GUIContent("Team 3"),
			new GUIContent("Team 4"),
		};
	}
}
