using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameStats : NetworkBehaviour {

	public static GameStats instance;

	public GameObject hudPrefab;
	
	TeamStats[] teams = new TeamStats[Globals.Players.TEAMS.Length];

	void Awake() {
		instance = this;
	}
	#region Client-side only methods

	[ClientRpc]
	public void RpcOnStartClient() {
		bool justSoloTeams = true;
		int numOfPlayers = 0;

		// Get teams
		Dictionary<int, List<Player>> teams = new Dictionary<int, List<Player>>();
		foreach(var p in FindObjectsOfType<Player>()) {
			if (!teams.ContainsKey(p.team)) teams[p.team] = new List<Player>();
			teams[p.team].Add(p);
			if (teams[p.team].Count > 1) justSoloTeams = false;
			numOfPlayers++;
		}

		List<int> order = new List<int>();
		for (int teamID = 1; teamID < Globals.Players.TEAMS.Length; teamID++)
			if (teams.ContainsKey(teamID)) order.Add(teamID);

		float weight = 0;
		for (int index = 0; index < order.Count; index++) {
			var team = teams[order[index]];

			// Spawn in one stats-set for each team
			var clone = Instantiate(hudPrefab) as GameObject;
			clone.transform.SetParent(transform, false);

			var rect = clone.GetComponent<RectTransform>();
			rect.pivot = new Vector2(.5f, 0);
			rect.anchorMin = new Vector2(weight, 0);
			rect.anchorMax = new Vector2(weight + team.Count / (float)numOfPlayers, 0);
			rect.offsetMin = new Vector2(5, 5);
			rect.offsetMax = new Vector2(-5, 55);
			weight += team.Count / (float)numOfPlayers;

			var stats = clone.GetComponent<TeamStats>();
			stats.teamIndex = order[index];
			Vector3 c = Vector3.zero;
			foreach (var p in team) {
				var color = Globals.Players.COLORS[p.player - 1];
				c += new Vector3(color.r, color.g, color.b);
			}
			c /= team.Count;
			stats.teamColor.color = new Color(c.x, c.y, c.z);
			stats.teamName.text = justSoloTeams ? "Player " + team[0].player : Globals.Players.TEAMS[order[index]].text;

			this.teams[order[index]] = stats;
		}
		
	}

	[ClientRpc]
	public void RpcSetScore(int team1, int team2, int team3, int team4) {
		if (teams.Length > 1 && teams[1])
			teams[1].teamScore.text = team1.ToString();

		if (teams.Length > 2 && teams[2])
			teams[2].teamScore.text = team2.ToString();

		if (teams.Length > 3 && teams[3])
			teams[3].teamScore.text = team3.ToString();

		if (teams.Length > 4 && teams[4])
			teams[4].teamScore.text = team4.ToString();
	}
	#endregion

}
