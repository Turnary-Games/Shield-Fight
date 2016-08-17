using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MyLobbyPlayer : NetworkLobbyPlayer {

	public int playerID = -1;
	public Popup inputPopup = new Popup();
	public Popup teamPopup = new Popup();
	[HideInInspector]
	[SyncVar]
	public int team;

	// This is a hook that is invoked on all player objects when entering the lobby.
	// Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.
	public override void OnClientEnterLobby() {
		var myLobbyManager = (NetworkManager.singleton as MyLobbyManager);
		myLobbyManager.isAddingPlayer = false;

		// Take the one that isn't taken
		List<int> freeInput = new List<int>();
		for (int i = 0; i < Globals.Input.DROPDOWN.Length; i++)
			freeInput.Add(i);

		for (int i = 0; i < myLobbyManager.lobbySlots.Length; i++) {
			var player = myLobbyManager.lobbySlots[i] as MyLobbyPlayer;
			if (player != null && player.isLocalPlayer && player != this) {
				freeInput.Remove(player.inputPopup.selectedItemIndex);
			}
		}

		inputPopup.selectedItemIndex = freeInput.Count > 0 ? freeInput[0] : 0;
	}

	// This is a hook that is invoked on all player objects when exiting the lobby.
	public override void OnClientExitLobby() {

	}

	// This is a hook that is invoked on clients when a LobbyPlayer switches between ready or not ready.
	// This function is called when the a client player calls SendReadyToBeginMessage() or SendNotReadyToBeginMessage().
	// > readyState: Whether the player is ready or not.
	public override void OnClientReady(bool readyState) {
		
	}

	
}
