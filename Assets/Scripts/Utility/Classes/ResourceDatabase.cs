using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResourceDatabase {

	public class PlayerResource : BaseResource {
		static Dictionary<int, PlayerResource> playerResources = new Dictionary<int, PlayerResource>();

		public readonly int PLAYER_ID;

		public readonly GameObject RESOURCE_CHARACTER_MODEL;
		public readonly GameObject RESOURCE_SHIELD_HELD_MODEL;
		public readonly GameObject RESOURCE_SHIELD_THROWN_MODEL;

		public readonly string PATH_BASE;
		public readonly string PATH_CHARACTER_MODEL;
		public readonly string PATH_SHIELD_HELD_MODEL;
		public readonly string PATH_SHIELD_THROWN_MODEL;

		public PlayerResource(int player) {
			PLAYER_ID = player;

			PATH_BASE = "Player resources/Player " + PLAYER_ID + " resources/";
			PATH_CHARACTER_MODEL = PATH_BASE + "Player model";
			PATH_SHIELD_HELD_MODEL = PATH_BASE + "Held shield model";
			PATH_SHIELD_THROWN_MODEL = PATH_BASE + "Thrown shield mOdel";

			RESOURCE_CHARACTER_MODEL = RESOURCE_CHARACTER_MODEL ?? LoadResource<GameObject>(PATH_CHARACTER_MODEL);
			RESOURCE_SHIELD_HELD_MODEL = RESOURCE_SHIELD_HELD_MODEL ?? LoadResource<GameObject>(PATH_SHIELD_HELD_MODEL);
			RESOURCE_SHIELD_THROWN_MODEL = RESOURCE_SHIELD_THROWN_MODEL ?? LoadResource<GameObject>(PATH_SHIELD_THROWN_MODEL);

			playerResources[PLAYER_ID] = this;
		}

		public static PlayerResource FetchPlayerResources(int player) {
			if (playerResources.ContainsKey(player))
				// Fetch existing resource
				return playerResources[player];
			else
				// Load resource
				return new PlayerResource(player);
		}
	}

	public abstract class BaseResource {
		public static Object LoadResource(string path) {
			Object obj = Resources.Load(path);
			if (obj == null) throw new System.Exception("Unable to load resource at \"" + path + "\"");
			return obj;
		}

		public static T LoadResource<T>(string path) where T : Object {
			T obj = Resources.Load<T>(path);
			if (obj == null) throw new System.Exception("Unable to load resource of type \"" + typeof(T).ToString() + "\" at \"" + path + "\"");
			return obj;
		}
	}
}
