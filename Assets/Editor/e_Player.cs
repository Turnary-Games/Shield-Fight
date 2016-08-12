using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects, CustomEditor(typeof(Player))]
public class e_Player : Editor {

	Player script;
	int oldPlayer;
	
	void OnEnable() {
		script = target as Player;
		oldPlayer = script.player;
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		bool isPrefab = PrefabUtility.GetPrefabParent(script.gameObject) == null && PrefabUtility.GetPrefabObject(script.gameObject) != null; // Is a prefab

		if (!isPrefab) {
			EditorGUILayout.Space();

			if (script.IsReset()) {
				if (GUILayout.Button("Spawn in model"))
					script.InitializePlayer();
			} else {
				if (GUILayout.Button("Remove model"))
					script.ResetPlayer();
				else
					EditorGUILayout.HelpBox("Remember to remove before applying to the prefab!", MessageType.Info);
			}
		}
		
		if (oldPlayer != script.player && !script.IsReset()) {
			script.InitializePlayer();
			oldPlayer = script.player;
		}
		
	}

}
