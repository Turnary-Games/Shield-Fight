using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LobbySettings : NetworkBehaviour {

	public static LobbySettings instance;
	
	[SyncVar]
	public bool customTeams;
	internal float numOfPlayers;

	void Awake() {
		instance = this;
	}
	

}
