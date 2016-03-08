using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkLobbyManager : NetworkLobbyManager
{
	// call on the server 
	public override void OnLobbyStartHost () {
		Debug.Log ("OnLobbyStartHost()");
	}

	public override void OnLobbyStopHost() {
		Debug.Log ("OnLobbyStopHost()");
	}

	public override void OnLobbyStartServer() {
		Debug.Log ("OnLobbyStartServer()");

	}

	void OnLobbyServerConnect() {
		Debug.Log ("OnLobbyServerConnect()");
	}

	void OnLobbyServerDisconnect() {
		Debug.Log ("OnLobbyServerDisconnect()");
	}

	void OnLobbyServerSceneChanged() {
		Debug.Log ("OnLobbyServerSceneChanged()");
	}

	void OnLobbyServerCreateLobbyPlayer() {
		Debug.Log ("OnLobbyServerCreateLobbyPlayer()");
	}

	void OnLobbyServerCreateGamePlayer() {
		Debug.Log ("OnLobbyServerCreateGamePlayer()");
	}

	void OnLobbyServerPlayerRemoved() {
		Debug.Log ("OnLobbyServerPlayerRemoved()");
	}

	void OnLobbyServerSceneLoadedForPlayer() {
		Debug.Log ("OnLobbyServerSceneLoadedForPlayer()");
	}

	public override void OnLobbyServerPlayersReady() {
		Debug.Log ("OnLobbyServerPlayersReady()");
		bool allReady = true;
		foreach (NetworkLobbyPlayer p in lobbySlots)
		{
			if (p == null || !p.readyToBegin)
				allReady = false;
		}
		if (allReady) {
			ServerChangeScene (this.playScene);
		}
	}


	// called on the client
	public override void OnLobbyClientEnter() {
		Debug.Log ("OnLobbyClientEnter()");
		base.OnLobbyClientEnter ();
	}

	public override void OnLobbyClientExit() {
		Debug.Log ("OnLobbyClientExit()");
		base.OnLobbyClientExit ();
	}

	void OnLobbyClientConnect() {
		Debug.Log ("OnLobbyClientConnect()");
	}

	void OnLobbyClientDisconnect() {
		Debug.Log ("OnLobbyClientDisconnect()");
	}

	void OnLobbyStartClient() {
		Debug.Log ("OnLobbyStartClient()");
	}

	public override void OnLobbyStopClient() {
		Debug.Log ("OnLobbyStopClient()");
		base.OnLobbyStopClient ();
	}

	void OnLobbyClientSceneChanged() {
		Debug.Log ("OnLobbyClientSceneChanged()");
	}

	public override void OnLobbyClientAddPlayerFailed() {
		Debug.Log ("OnLobbyClientAddPlayerFailed()");
		base.OnLobbyClientAddPlayerFailed ();
	}
}
