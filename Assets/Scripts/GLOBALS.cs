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
	}

}
