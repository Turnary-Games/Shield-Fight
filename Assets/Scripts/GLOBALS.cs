using UnityEngine;
using System.Collections;
using System;

namespace Globals {

	public class Game {
		public static readonly Version GAME_VERSION = new Version(0, 1, 0);
	}

	public class Player {
		public const int NUM_OF_PLAYERS = 4;

		public static int GetLayerHeldShield(int player) { return LayerMask.NameToLayer("Shield " + player); }
		public static int GetLayerThrownShield(int player) { return LayerMask.NameToLayer("Player " + player); }
		public static int GetLayerPlayer(int player) { return LayerMask.NameToLayer("Player " + player); }

		public static string GetInputHoriztonalName(int player) { return "P" + player + " Horizontal"; }
		public static string GetInputVerticalName(int player) { return "P" + player + " Vertical"; }
		public static string GetInputFireName(int player) { return "P" + player + " Fire"; }
		public static string GetInputPushName(int player) { return "P" + player + " Push"; }

		public const string PATH_RESOURCES = "Player resources/";
		public static string GetPathPlayerBase(int player) { return PATH_RESOURCES + "Player " + player + " resources/"; }
		public static string GetPathPlayerCharacterModel(int player) { return GetPathPlayerBase(player) + "Player model"; }
		public static string GetPathPlayerShieldModel(int player) { return GetPathPlayerBase(player) + "Shield model"; }
		public static string GetPathPlayerShieldMaterial(int player) { return GetPathPlayerBase(player) + "Shield material"; }

	}

}
