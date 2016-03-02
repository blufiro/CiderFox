using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
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

	public MyNetworkLobbyManager networkLobbyManager;

	private NetworkMatch match;
	private MatchDesc selectedMatchDesc;
	private State currState;

	void Start() {
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
		switchState(State.ONLINE);
		networkLobbyManager.StartMatchMaker();
		match = networkLobbyManager.matchMaker;
	}

	public void OnClickOnlineHost() {
		switchState (State.ONLINE_HOST);
	}

	public void OnClickOnlineFind() {
		switchState (State.ONLINE_FIND);
	}

	public void OnClickCreateGame() {
		var req = new CreateMatchRequest();
		req.name = onlineHostGameNameInput.text;
		req.size = G.NUM_PLAYERS;
		req.advertise = onlineHostGamePublicToggle.isOn;
		req.password = onlineHostGamePasswordInput.text;
		match.CreateMatch(req, networkLobbyManager.OnMatchCreate);
		switchState (State.LOBBY);
	}

	public void OnClickFindGame() {
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

	void OnMatchList(ListMatchResponse matchList)
	{
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
		}
		Debug.Log("match count: " + matchList.matches.Count);
	}

	public void OnClickSelectGame(MatchDesc selectedMatch) {
		selectedMatchDesc = selectedMatch;
		onlineFindJoinGameOverlay.SetActive (true);
		onlineFindJoinGameNameText.text = selectedMatchDesc.name;
	}

	public void OnClickBackOnJoinGameOverlay() {
		selectedMatchDesc = null;
		onlineFindJoinGameOverlay.SetActive (false);
	}

	public void OnClickJoinGame() {
		var req = new JoinMatchRequest();
		req.networkId = selectedMatchDesc.networkId;
		req.password = onlineFindPasswordInput.text;
		match.JoinMatch(req, networkLobbyManager.OnMatchJoined);
		switchState (State.LOBBY);
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
			Debug.Log ("StopMatchMaker called!");
			networkLobbyManager.StopMatchMaker();
			match = null;
		}
	}

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
	private void switchState(State state) {
		titleMenu.SetActive(false);
		networkMenu.SetActive(false);
		lanMenu.SetActive(false);
		onlineMenu.SetActive(false);
		onlineHostMenu.SetActive(false);
		onlineFindMenu.SetActive(false);
		lobbyMenu.SetActive(false);
		creditsMenu.SetActive(false);
		switch (state) {
			case State.TITLE: titleMenu.SetActive(true); break;
			case State.NETWORK: networkMenu.SetActive(true); break;
			case State.LAN: lanMenu.SetActive(true); break;
			case State.ONLINE: onlineMenu.SetActive(true); break;
			case State.ONLINE_HOST: onlineHostMenu.SetActive(true); break;
			case State.ONLINE_FIND: onlineFindMenu.SetActive(true); OnClickFindGame (); break;
			case State.LOBBY: lobbyMenu.SetActive (true); break;
			case State.CREDITS: creditsMenu.SetActive(true); break;
		}
		currState = state;
	}
}
