using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class GameCountdown : NetworkBehaviour {
	
	public static GameCountdown instance;
	
	public Image backgroundPanel;
	public Color backgroundColor = new Color(0,0,0,.333f);
	[Header("Waiting for players...")]
	public Text waitingText;
	public Text waitingDots;
	[Header("Game starts in...")]
	public Text gameStartsText;
	public Text gameStartsTime;
	public float gameStartDelay = 5;
	public float gameStartsTimeBig = 300;
	public float gameStartsTimeSmall = 150;
	[Header("GOOOOOO")]
	public Text goooooText;
	public float goooooDelay = 1;
	public float goooooTextBig = 300;
	public float goooooTextSmall = 100;

	private State state = State.waitingForPlayers;
	private float start;

	void Awake() {
		instance = this;
	}

	void Start() {
		start = Time.time;
		waitingDots.enabled =
		waitingText.enabled =
		backgroundPanel.enabled = true;
		backgroundPanel.color = backgroundColor;
	}

	void Update() {
		if (state == State.waitingForPlayers) {
			if (isClient)
				waitingDots.text = new string('.', Mathf.FloorToInt((Time.time - start)*2) % 4);
		} else if (state == State.countdown) {
			float timeLeft = gameStartDelay - Time.time + start;

			if (timeLeft > 0) {
				if (isClient) {
					gameStartsTime.text = Mathf.CeilToInt(timeLeft).ToString();
					gameStartsTime.fontSize = Mathf.FloorToInt(Mathf.Lerp(gameStartsTimeSmall, gameStartsTimeBig, timeLeft % 1));
					gameStartsTime.color = Color.white;
				}
			} else {
				state = State.wait;
				if (isServer)
					CmdStartGame();
			}

		} else if (state == State.go) {
			float timeLeft = goooooDelay - Time.time + start;
			if (timeLeft > 0) {
				if (isClient) {
					Color col = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, Mathf.Lerp(backgroundColor.a, 0, timeLeft % 1));
					backgroundPanel.color = col;
					goooooText.color = Color.white;
					goooooText.fontSize = Mathf.FloorToInt(Mathf.Lerp(goooooTextSmall, goooooTextBig, timeLeft % 1));
					col.r = col.g = col.b = 1;
					goooooText.color = col;
				}
			} else {
				state = State.wait;
				if (isClient) {
					backgroundPanel.enabled = false;
					goooooText.enabled = false;
					backgroundPanel.color = backgroundColor;
				}
			}
		}
	}

	#region Server-side only methods
	[Command]
	public void CmdStartGame() {
		RpcStartGame();

		foreach (var player in FindObjectsOfType<Player>()) {
			player.RpcStartGame();
		}
	}

	[Command]
	public void CmdStartCountdown() {
		RpcStartCountdown();
	}
	#endregion

	#region Client-side only methods
	[ClientRpc]
	public void RpcStartCountdown() {
		state = State.countdown;
		start = Time.time;
		waitingText.enabled =
		waitingDots.enabled = false;
		gameStartsText.enabled =
		gameStartsTime.enabled = true;
		gameStartsTime.color = Color.clear;
	}

	[ClientRpc]
	void RpcStartGame() {
		state = State.go;
		start = Time.time;
		gameStartsText.enabled =
		gameStartsTime.enabled = false;
		goooooText.enabled = true;
		goooooText.color = Color.clear;
	}
	#endregion
	
	enum State {
		waitingForPlayers,
		countdown,
		go,
		wait
	}
}
