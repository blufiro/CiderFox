﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using System.Collections.Generic;

public class TitleScreen : MonoBehaviour {

	public GameObject titleMenu;
	public GameObject networkMenu;
	public GameObject lanMenu;
	public GameObject onlineMenu;
	public GameObject onlineHostMenu;
	public GameObject onlineFindMenu;
	public GameObject lobbyMenu;
	public GameObject creditsMenu;

	// UI on onlineHostMenu
	public InputField onlineHostGameNameInput;
	public InputField onlineHostGamePasswordInput;
	public Toggle onlineHostGamePublicToggle;

	// UI on onlineFindMenu
	public InputField onlineFindFilterGameNameInput;
	public Button onlineFindGameButton;
	public ScrollRect onlineFindScrollRect;
	public GameObject matchButtonTemplate;
	public int buttonSpace;
	public GameObject onlineFindJoinGameOverlay;
	public Text onlineFindJoinGameNameText;
	public InputField onlineFindPasswordInput;

	enum State {
		TITLE,
		NETWORK, // LAN or Online
		LAN, // Host or Client
		ONLINE, // Host or Find game
		ONLINE_HOST, // Host game
		ONLINE_FIND, // Find game
		LOBBY, // Lobby
		CREDITS,
	}
	private MyNetworkLobbyManager networkLobbyManager;
	private NetworkMatch match;
	private MatchDesc selectedMatchDesc;
	private long networkId;
	private long nodeId;
	private bool isMatchCreator;
	private State nextState;

	void Start() {
		// networkLobbyManager is not destroyed on load, so we have to find it
		// in case this title screen is destroyed.
		networkLobbyManager = GameObject.Find("NetworkManager").GetComponent<MyNetworkLobbyManager>();
		switchState(State.TITLE);
	}

	public void OnClickStart() {
		// switchState(State.NETWORK);
		OnClickOnline ();
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
		Debug.Log ("OnClickOnline()");
		cleanUpMatchAndSwitchState(State.ONLINE);
		Debug.Log ("networkLobbyManager: " + networkLobbyManager);
	}

	public void OnSwitchToOnline() {
		Debug.Log ("OnSwitchToOnline()");
		networkLobbyManager.StartMatchMaker();
		match = networkLobbyManager.matchMaker;
	}

	public void OnClickOnlineHost() {
		Debug.Log ("OnClickOnlineHost()");
		switchState (State.ONLINE_HOST);
	}

	public void OnClickOnlineFind() {
		Debug.Log ("OnClickOnlineFind()");
		switchState (State.ONLINE_FIND);
	}

	public void OnClickCreateGame() {
		Debug.Log ("OnClickCreateGame()");
		var req = new CreateMatchRequest();
		req.name = onlineHostGameNameInput.text;
		req.size = G.NUM_PLAYERS;
		req.advertise = onlineHostGamePublicToggle.isOn;
		req.password = onlineHostGamePasswordInput.text;
		match.CreateMatch(req, this.OnMatchCreate);
		isMatchCreator = true;
		switchState (State.LOBBY);
	}

	void OnMatchCreate(CreateMatchResponse matchInfo) {
		Debug.Log ("OnMatchCreate()");
		networkLobbyManager.OnMatchCreate (matchInfo);
		this.networkId = (long) matchInfo.networkId;
		this.nodeId = (long) matchInfo.nodeId;
	}

	public void OnClickFindGame() {
		Debug.Log ("OnClickFindGame()");
		var req = new ListMatchRequest ();
		req.pageNum = 0;
		req.pageSize = 20;
		req.nameFilter = onlineFindFilterGameNameInput.text;
		req.includePasswordMatches = false;
		match.ListMatches(req, OnMatchList);
		// clear content
		for (int i = 0; i < onlineFindScrollRect.content.childCount; i++) {
			Transform t = onlineFindScrollRect.content.GetChild (i);
			t = null;
			Destroy (t);
		}
		onlineFindGameButton.gameObject.SetActive (false);
	}

	void OnMatchList(ListMatchResponse matchList) {
		Debug.Log ("OnMatchList()");
		onlineFindGameButton.gameObject.SetActive (true);
		networkLobbyManager.OnMatchList (matchList);
		if (matchList.matches.Count > 0) {
			// add matches as buttons
			int j = 0;
			Debug.Log (onlineFindScrollRect.content);
			Debug.Log (onlineFindScrollRect.content.transform);
			foreach (MatchDesc matchDesc in matchList.matches) {
				GameObject matchButton = (GameObject) Instantiate (matchButtonTemplate);
				matchButton.transform.SetParent(onlineFindScrollRect.content.transform, false);
				matchButton.transform.Translate(new Vector3 (0, -j * buttonSpace, 0));
				matchButton.transform.GetChild (0).GetComponent<Text> ().text = matchDesc.name;
				matchButton.GetComponent<Button> ().onClick.AddListener (
					delegate { OnClickSelectGame (matchDesc); });
				j++;
			}
			onlineFindScrollRect.content.sizeDelta =
				new Vector2(onlineFindScrollRect.content.sizeDelta.x, j * buttonSpace);
		}
		Debug.Log("match count: " + matchList.matches.Count);
	}

	public void OnClickSelectGame(MatchDesc selectedMatch) {
		Debug.Log ("OnClickSelectGame()");
		selectedMatchDesc = selectedMatch;
		onlineFindJoinGameOverlay.SetActive (true);
		onlineFindJoinGameNameText.text = selectedMatchDesc.name;
	}

	public void OnClickBackOnJoinGameOverlay() {
		Debug.Log ("OnClickBackOnJoinGameOverlay()");
		selectedMatchDesc = null;
		onlineFindJoinGameOverlay.SetActive (false);
	}

	public void OnClickJoinGame() {
		Debug.Log ("OnClickJoinGame()");
		var req = new JoinMatchRequest();
		req.networkId = selectedMatchDesc.networkId;
		req.password = onlineFindPasswordInput.text;
		match.JoinMatch(req, this.OnMatchJoined);
		isMatchCreator = false;
		switchState (State.LOBBY);
	}

	void OnMatchJoined(JoinMatchResponse matchInfo) {
		Debug.Log ("OnMatchJoined()");
		networkLobbyManager.OnMatchJoined (matchInfo);
		this.networkId = (long) matchInfo.networkId;
		this.nodeId = (long) matchInfo.nodeId;
	}

	public void OnClickCredits() {
		switchState(State.CREDITS);
	}

	public void OnClickQuit() {
		Application.Quit();
	}

	public void OnClickBackToTitle() {
		Debug.Log ("OnClickBackToTitle()");
		cleanUpMatchAndSwitchState(State.TITLE);
	}

	void OnDestroyMatch(BasicResponse response) {
		Debug.Log ("OnDestroyMatch()");
		networkLobbyManager.StopClient ();
		NetworkServer.Reset();
		match = null;
		switchState(nextState);
	}

	void OnConnectionDrop(BasicResponse response) {
		Debug.Log ("OnConnectionDrop()");
		networkLobbyManager.StopClient ();
		NetworkServer.Reset();
		match = null;
		switchState(nextState);
	}

	private void OffAllUI() {
		titleMenu.SetActive(false);
		networkMenu.SetActive(false);
		lanMenu.SetActive(false);
		onlineMenu.SetActive(false);
		onlineHostMenu.SetActive(false);
		onlineFindMenu.SetActive(false);
		onlineFindJoinGameOverlay.SetActive (false);
		lobbyMenu.SetActive(false);
		creditsMenu.SetActive(false);
	}
	private void switchState(State state) {
		Debug.Log("switchState: " + state);
		OffAllUI();
		switch (state) {
			case State.TITLE: titleMenu.SetActive(true); break;
			case State.NETWORK: networkMenu.SetActive(true); break;
			case State.LAN: lanMenu.SetActive(true); break;
			case State.ONLINE: onlineMenu.SetActive(true); OnSwitchToOnline(); break;
			case State.ONLINE_HOST: onlineHostMenu.SetActive(true); break;
			case State.ONLINE_FIND: onlineFindMenu.SetActive(true); OnClickFindGame (); break;
			case State.LOBBY: lobbyMenu.SetActive (true); break;
			case State.CREDITS: creditsMenu.SetActive(true); break;
		}
	}

	private void cleanUpMatchAndSwitchState(State state) {
		if (match != null) {
			OffAllUI();
			// if match is active, we only switch to new state on callbacks, otherwise, they may be cancelled.
			nextState = state;
			if (isMatchCreator) {
				Debug.Log ("isMatchCreator, DestroyMatch");
				networkLobbyManager.matchMaker.DestroyMatch ((NetworkID)networkId, OnDestroyMatch);
			} else{
				Debug.Log ("not isMatchCreator, DropConnection");
				DropConnectionRequest dropReq = new DropConnectionRequest ();
				dropReq.networkId = (NetworkID) networkId;
				dropReq.nodeId = (NodeID) nodeId;
				networkLobbyManager.matchMaker.DropConnection (dropReq, OnConnectionDrop);
			}
		} else {
			switchState(state);
		}
	}
}
