using UnityEngine;
using System.Collections;
using System;

namespace Globals {

	public class Game {
		public static readonly Version GAME_VERSION = new Version(0, 1, 0);
	}

	public class Player {
		public const int NUM_OF_PLAYERS = 4;

		public const string INPUT_PREFIX = "P";
		public const string INPUT_HORIZONTAL_SUFFIX = " Horizontal";
		public const string INPUT_VERTICAL_SUFFIX = " Vertical";
		public const string INPUT_FIRE_SUFFIX = " Fire";
		public const string INPUT_PUSH_SUFFIX = " Push";


		public static readonly Color[] COLORS = {
		/* Player 1 */	Color.yellow,
		/* Player 2 */	Color.magenta,
		/* Player 3 */	Color.blue,
		/* Player 4 */	Color.green
		};

		public const string RESOURCES_PATH = "Player resources";
		public static string Get
		public enum MODELS {
			Cube,
			Sphere,
		}
	}

}
