using UnityEngine;
using System.Collections;
using System;

namespace Globals {

	public class Game {
		public static readonly Version VERSION = new Version(0, 2, 0);
	}

	public class Input {
		public static readonly GUIContent[] DROPDOWN = new GUIContent[] {
			new GUIContent("WASD + Mouse"),
			new GUIContent("Xbox 360 controller 1"),
			new GUIContent("Xbox 360 controller 2"),
			new GUIContent("Xbox 360 controller 3"),
			new GUIContent("Xbox 360 controller 4"),
		};
	}

	public class Players {
		public static readonly Color[] COLORS = new Color[] {
			Color.yellow,
			Color.blue,
			Color.green,
			Color.red,
		};
	}
}
