using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MyLobbyManager : NetworkLobbyManager {
	
	public enum LobbyState {
		Disconnected,
		Hosting,
		Visiting,
	}

	LobbyState state = LobbyState.Disconnected;
	[System.NonSerialized]
	public bool isAddingPlayer = false;

	GUIStyle boxStyle;
	GUIStyle listStyle;

	void Start() {
		listStyle = new GUIStyle();
		listStyle.normal.textColor = Color.white;
		var tex = new Texture2D(1, 1);
		tex.SetPixel(1, 1, Color.white);
		tex.Apply();
		listStyle.hover.background = tex;
		listStyle.onHover.background = tex;
		listStyle.padding = new RectOffset(4, 4, 4, 4);
	}

	void OnGUI() {
		GUI.Label(new Rect(200, 175, 100, 50), state.ToString());
		if (state == LobbyState.Hosting || state == LobbyState.Visiting) {
			var style = new GUIStyle();
			style.alignment = TextAnchor.MiddleCenter;

			float w = 150;
			int emptySlot = -1;

			for (int i = 0; i < lobbySlots.Length; i++) {
				var player = lobbySlots[i] as MyLobbyPlayer;
				if (player) {
					if (i >= 0 && i <= 3) {
						var s = new GUIStyle();
						var tex = new Texture2D(1, 1);
						tex.SetPixel(1, 1, Globals.Players.COLORS[i]);
						tex.Apply();
						s.normal.background = tex;

						GUI.Box(new Rect(220 + i * w, 350, w - 40, w - 40), GUIContent.none, s);
					}

					style.normal.textColor = player.isLocalPlayer ? Color.green : Color.cyan;
					GUI.Label(new Rect(200 + i * w, 200, w, 20), "<Player " + (i + 1)+">", style);

					if (player.isLocalPlayer) {
						if (GUI.Button(new Rect(200 + i * w, 240, w, 20), player.readyToBegin ? "Is ready!" : "Isn't ready")) {
							if (player.readyToBegin) player.SendNotReadyToBeginMessage();
							else player.SendReadyToBeginMessage();
						}

						boxStyle = new GUIStyle(GUI.skin.button);
						
						if (LobbySettings.instance.customTeams) {
							int lastVal = player.teamPopup.selectedItemIndex;
							player.teamPopup.List(new Rect(200 + i * w, 280, w, 20), Globals.Players.TEAMS, boxStyle, listStyle);
							player.team = player.teamPopup.selectedItemIndex;
							// Team changed, tell erryone to readyup again
							if (lastVal != player.team) {
								for (int j=0; j<lobbySlots.Length; j++) {
									if (lobbySlots[j])
										lobbySlots[j].SendNotReadyToBeginMessage();
								}
							}
						}
						
						player.inputPopup.List(new Rect(200 + i * w, 260, w, 20), Globals.Input.DROPDOWN, boxStyle, listStyle);
					} else {
						style.normal.textColor = player.readyToBegin ? Color.green : Color.red;
						GUI.Label(new Rect(200 + i * w, 240, w, 20), player.readyToBegin ? "Is ready!" : "Isn't ready", style);
						if (LobbySettings.instance.customTeams)
							GUI.Label(new Rect(200 + i * w, 280, w, 20), Globals.Players.TEAMS[player.team]);
					}

					if (player.isLocalPlayer || player.isServer) {
						GUI.color = Color.white;
						if (GUI.Button(new Rect(200 + i * w, 220, w, 20), player.isLocalPlayer ? "Leave" : "Kick"))
							player.RemovePlayer();
					}
				} else {
					if (emptySlot == -1)
						emptySlot = i;

					style.normal.textColor = Color.red;
					GUI.Label(new Rect(200 + i * w, 200, w, 20), "<Empty slot>", style);
					GUI.color = Color.white;
				}
			}
			if (isAddingPlayer)
				GUI.Label(new Rect(320, 175, 100, 20), "Adding player...");
			else {
				if (emptySlot != -1) {
					GUI.backgroundColor = Globals.Players.COLORS[emptySlot];
					if (GUI.Button(new Rect(320, 175, 100, 20), "Add player")) {
						isAddingPlayer = true;
						TryToAddPlayer();
					}
					GUI.backgroundColor = Color.white;
				}
			}
			if (state == LobbyState.Hosting) {
				if (GUI.Button(new Rect(440, 175, 100, 20), "Teams: " + (LobbySettings.instance.customTeams ? "On" : "Off")))
					LobbySettings.instance.customTeams = !LobbySettings.instance.customTeams;
			} else {
				GUI.Label(new Rect(440, 175, 100, 20), "Teams: " + (LobbySettings.instance.customTeams ? "On" : "Off"), style);
			}
			
		}
	}

	#region Called by server
	// This is called on the host when a host is started.
	public override void OnLobbyStartHost() {
		
	}
	// This is called on the host when the host is stopped.
	public override void OnLobbyStopHost() {
		state = LobbyState.Disconnected;
	}
	// This is called on the server when the server is started - including when a host is started.
	public override void OnLobbyStartServer() { 
		state = LobbyState.Hosting;
	}
	// This is called on the server when a new client connects to the server.
	// > conn: The new connection.
	public override void OnLobbyServerConnect(NetworkConnection conn) {

	}
	// This is called on the server when a client disconnects.
	// > conn: The connection that disconnected.
	public override void OnLobbyServerDisconnect(NetworkConnection conn) {

	}
	// This is called on the server when a networked scene finishes loading.
	// > sceneName: Name of the new scene.
	public override void OnLobbyServerSceneChanged(string sceneName) {

	}
	// This allows customization of the creation of the lobby-player object on the server.
	// By default the lobbyPlayerPrefab is used to create the lobby-player, but this function allows that behaviour to be customized.
	// > conn: The connection the player object is for.
	// > playerControllerId: The controllerId of the player.
	// < return GameObject: The new lobby-player object.
	public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId) {
		return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
	}
	// This allows customization of the creation of the GamePlayer object on the server.
	// By default the gamePlayerPrefab is used to create the game-player, but this function allows that behaviour to be customized.
	// The object returned from the function will be used to replace the lobby-player on the connection.
	// > conn: The connection the player object is for.
	// > playerControllerId: The controllerId of the player.
	// < return GameObject: A new GamePlayer object.
	public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId) {
		return base.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
	}
	// This is called on the server when a player is removed.
	// > conn: The connection the player object is for.
	// > playerControllerId: The controllerId of the player.
	public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId) {
		
	}
	// This is called on the server when it is told that a client has finished switching from the lobby scene to a game player scene.
	// When switching from the lobby, the lobby-player is replaced with a game-player object. This callback function gives an opportunity
	// to apply state from the lobby-player to the game-player object.
	// > lobbyPlayer: The lobby player object.
	// > gamePlayer: The game player object.
	// < return bool: False to not allow this player to replace the lobby player.
	private int serverPlayerID;
	private int serverHost;
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer) {
		var gamePlayerScript = gamePlayer.GetComponent<Player>();
		var lobbyPlayerScript = lobbyPlayer.GetComponent<MyLobbyPlayer>();

		gamePlayerScript.HOST_ID = serverHost;
		gamePlayerScript.player = lobbyPlayerScript.playerID;
		gamePlayerScript.input = lobbyPlayerScript.inputPopup.selectedItemIndex + 1;

		gamePlayerScript.transform.position = SpawnPoints.GetSpawningPosition(gamePlayerScript.player, numPlayers).new_y(1);
		
		gamePlayerScript.team = lobbyPlayerScript.teamPopup.selectedItemIndex;

		return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
	}
	// This is called on the server when all the players in the lobby are ready.
	// The default implementation of this function uses ServerChangeScene() to switch to the game player scene.
	// By implementing this callback you can customize what happens when all the players in the lobby are ready,
	// such as adding a countdown or a confirmation for a group leader.
	public override void OnLobbyServerPlayersReady() {
		// Make sure EVERYONE is ready...
		for (int i = 0; i < lobbySlots.Length; i++)
			if (lobbySlots[i] != null && !lobbySlots[i].readyToBegin) return;

		serverPlayerID = 0;
		serverHost = 0;
		Player.playersLoaded = new bool[numPlayers];

		// Take the one that isn't taken
		List<int> freeTeam = new List<int>();
		int[] usedTeams = new int[Globals.Players.TEAMS.Length];
		for (int i = 1; i < Globals.Players.TEAMS.Length; i++)
			freeTeam.Add(i);

		for (int i = 0; i < lobbySlots.Length; i++) {
			var player = lobbySlots[i] as MyLobbyPlayer;
			if (player != null) {
				freeTeam.Remove(player.team);
			}
		}

		// Check whos the host
		for (int i = 0; i < lobbySlots.Length; i++) {
			if (lobbySlots[i] != null) {
				var p = lobbySlots[i] as MyLobbyPlayer;

				p.playerID = serverPlayerID = serverPlayerID + 1;
				if (serverHost == 0 && p.isLocalPlayer) serverHost = p.playerID;

				// If team == <No team> then get an unique team
				if (p.team == 0) {
					p.team = freeTeam.Count > 0 ? freeTeam.Pop(0) : 1;
				}
				p.teamPopup.selectedItemIndex = p.team;

				usedTeams[p.team]++;
			}
		}

		// Check so we don't have everyone on same team
		for (int i=0; i<usedTeams.Length; i++) {
			if (usedTeams[i] == numPlayers)
				return;
		}
		
		ServerChangeScene(playScene);
	}

	// Called on the server when a client adds a new player with ClientScene.AddPlayer.
	// The default implementation for this function creates a new player object from the playerPrefab.
	// > conn: Connection from client.
	// > playerControllerId: Id of the new player.
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		base.OnServerAddPlayer(conn, playerControllerId);

		for (int i = 0; i < lobbySlots.Length; i++) {
			var player = lobbySlots[i] as MyLobbyPlayer;
			if (player && player.readyToBegin) player.SendNotReadyToBeginMessage();
		}
	}
	#endregion

	#region Client-side only methods
	// This is a hook to allow custom behaviour when the game client enters the lobby.
	public override void OnLobbyClientEnter() {
		if (state != LobbyState.Hosting)
			state = LobbyState.Visiting;
	}
	// This is a hook to allow custom behaviour when the game client exits the lobby.
	public override void OnLobbyClientExit() {
		state = LobbyState.Disconnected;
	}
	// This is called on the client when it connects to server.
	// > conn: The connection that connected.
	public override void OnLobbyClientConnect(NetworkConnection conn) {

	}
	// This is called on the client when disconnected from a server.
	// > conn: The connection that disconnected.
	public override void OnLobbyClientDisconnect(NetworkConnection conn) {

	}
	// This is called on the client when a client is started.
	// > lobbyClient: The network client class.
	public override void OnLobbyStartClient(NetworkClient lobbyClient) {
		
	}
	// This is called on the client when the client stops.
	public override void OnLobbyStopClient() {
		
	}
	// This is called on the client when the client is finished loading a new networked scene.
	// > conn: The connection.
	public override void OnLobbyClientSceneChanged(NetworkConnection conn) {
		
	}
	// Called on the client when adding a player to the lobby fails.
	// This could be because the lobby is full, or the connection is not allowed to have more players.
	public override void OnLobbyClientAddPlayerFailed() {
		isAddingPlayer = false;
		print("Failed to add player!");
	}
	#endregion
}
