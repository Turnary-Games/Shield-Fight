using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameMatchPoint : NetworkBehaviour {
	public static GameMatchPoint instance;

	private int deadPlayers = 0;
	private int[] teamScores = new int[Globals.Players.TEAMS.Length];

	void Awake() {
		instance = this;
	}

	bool HasSomeoneAlive(int team) {
		foreach (Player p in FindObjectsOfType<Player>()) {
			if (p.team == team && !p.dead)
				return true;
		}
		return false;
	}

	#region Server-side only methods
	[Command]
	public void CmdOnPlayerDied(int playerID) {
		Player player = Player.GetFromPlayerID(playerID);
		if (player == null || !player.dead) return;

		for (int i = 1; i < teamScores.Length; i++) {
			if (player.team != i && HasSomeoneAlive(i)) {
				teamScores[i]++;
			}
		}

		GameStats.instance.RpcSetScore(teamScores[1], teamScores[2], teamScores[3], teamScores[4]);

		deadPlayers++;

		if (deadPlayers == FindObjectsOfType<Player>().Length - 1) {
			deadPlayers = 0;

			foreach(Player p in FindObjectsOfType<Player>()) {

				p.transform.position = SpawnPoints.GetSpawningPosition(p.player, FindObjectsOfType<Player>().Length).new_y(1);
				p.dead = true;
				p.body.velocity = Vector3.zero;

				p.RpcJumpToSpawnpoint();
				p.CmdPickupShield();
				GameCountdown.instance.CmdStartCountdown();
			}
			foreach(Shield s in FindObjectsOfType<Shield>()) {
				NetworkServer.Destroy(s.gameObject);
			}
		}
	}
	#endregion

}
