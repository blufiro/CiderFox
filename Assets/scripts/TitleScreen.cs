using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	public GameObject titleMenu;
	public GameObject networkMenu;
	public GameObject lanMenu;
	public GameObject onlineLobbyMenu;
	public GameObject creditsMenu;

	public NetworkLobbyManager networkLobbyManager;

	private NetworkMatch match;

	void Start() {
		switchState(State.TITLE);
	}

	public void OnClickStart() {
		switchState(State.NETWORK);
	}

	public void OnClickLAN() {
		switchState(State.LAN);
	}

	public void OnClickLANHost() {
		Debug.LogError("TODO");
	}

	public void OnClickLANClient() {
		Debug.LogError("TODO");
	}

	public void OnClickOnline() {
		switchState(State.ONLINE);
		networkLobbyManager.StartMatchMaker();
		match = networkLobbyManager.matchMaker;
	}

	public void OnClickCreateGame() {
		var req = new CreateMatchRequest();
		req.name = "name";
		req.size = 4;
		req.advertise = true;
		req.password = "";
		match.CreateMatch(req, networkLobbyManager.OnMatchCreate);
	}

	public void OnClickFindGame() {
		var req = new ListMatchRequest ();
		req.pageNum = 0;
		req.pageSize = 20;
		req.nameFilter = "";
		req.includePasswordMatches = false;
		match.ListMatches(req, OnMatchList);
	}

	void OnMatchList(ListMatchResponse matchList)
	{
		networkLobbyManager.OnMatchList (matchList);
		if (matchList.matches.Count > 0) {
			var matchDesc = matchList.matches [0];
			var req = new JoinMatchRequest();
			req.networkId = matchDesc.networkId;
			req.password = "";
			match.JoinMatch(req, networkLobbyManager.OnMatchJoined);
		}
		Debug.Log("match count: " + matchList.matches.Count);
	}

	public void OnClickJoinGame() {
		var req = new JoinMatchRequest();
		match.JoinMatch(req, networkLobbyManager.OnMatchJoined);
	}

	public void OnClickCredits() {
		switchState(State.CREDITS);
	}

	public void OnClickQuit() {
		Application.Quit();
	}

	public void OnClickBackToTitle() {
		switchState(State.TITLE);
		if (match != null) {
			networkLobbyManager.StopMatchMaker();
			match = null;
		}
	}

	enum State {
		TITLE,
		NETWORK, // LAN or Online
		LAN, // Host or Client
		ONLINE, // Create game or Find game
		CREDITS,
	}
	private void switchState(State state) {
		titleMenu.SetActive(false);
		networkMenu.SetActive(false);
		lanMenu.SetActive(false);
		onlineLobbyMenu.SetActive(false);
		creditsMenu.SetActive(false);
		switch (state) {
			case State.TITLE: titleMenu.SetActive(true); break;
			case State.NETWORK: networkMenu.SetActive(true); break;
			case State.LAN: lanMenu.SetActive(true); break;
			case State.ONLINE: onlineLobbyMenu.SetActive(true); break;
			case State.CREDITS: creditsMenu.SetActive(true); break;
		}
	}
}
