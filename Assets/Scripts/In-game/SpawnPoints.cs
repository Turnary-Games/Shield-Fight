using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpawnPoints : SingletonBase<SpawnPoints> {

	[Header("1v1")]
	public Transform[] oneVsOne;
	[Header("2v1")]
	public Transform[] twoVsOneTeam1;
	public Transform[] twoVsOneTeam2;
	[Header("3v1")]
	public Transform[] threeVsOneTeam1;
	public Transform[] threeVsOneTeam2;
	[Header("1v1v1")]
	public Transform[] threeFFA;
	[Header("1v1v1v1")]
	public Transform[] fourFFA;
	[Header("2v2")]
	public Transform[] twoVsTwoTeam1;
	public Transform[] twoVsTwoTeam2;

	public static Vector3 GetSpawningPosition(int playerID, int numOfPlayers) {
		if (numOfPlayers == 2) {
			// 1v1
			return instance.oneVsOne[playerID - 1].position;
		} else if (numOfPlayers == 3) {
			// 1v1v1
			return instance.threeFFA[playerID - 1].position;
		} else if (numOfPlayers == 4) {
			// 1v1v1v1
			return instance.fourFFA[playerID - 1].position;
		} else {
			return Vector3.zero;
		}
	}

}
