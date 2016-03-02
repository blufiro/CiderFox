using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class LobbyPlayerBehaviour : NetworkLobbyPlayer {

	public GameObject lobbyUITemplate;

	private GameObject lobbyUI;
	private Toggle readyToggle;

//	public override void OnStartServer() {
//		Debug.Log ("lobbyplayer.OnStartServer");
//	}

	public override void OnStartClient() {
		base.OnStartClient ();
		Debug.Log ("lobbyplayer.OnStartClient");
		GameObject canvas = GameObject.Find ("Canvas");

		if (lobbyUI != null) {
			Destroy (lobbyUI);
			lobbyUI = null;
		}
		lobbyUI = (GameObject)Instantiate (lobbyUITemplate);
		readyToggle = lobbyUI.transform.GetComponentInChildren<Toggle> ();
		lobbyUI.transform.SetParent(canvas.transform, false);
		float width = lobbyUI.GetComponent<RectTransform> ().sizeDelta.x;
		lobbyUI.transform.localPosition = (new Vector3 ( (this.slot == 0) ? -width : width, 0));

		readyToggle.interactable = false;
	}

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer ();
		Debug.Log ("lobbyplayer.OnStartLocalPlayer");
		readyToggle.interactable = true;
		readyToggle.onValueChanged.AddListener(OnClickReadyToggle);
	}

//	public override void OnClientEnterLobby() {
//		base.OnClientEnterLobby
//		readyToggle.enabled = this.isLocalPlayer;
//	}
//
//	public override void OnClientExitLobby() {
//		lobbyUI.SetActive (false);
//	}

	public override void OnClientReady(bool readyState) {
		readyToggle.isOn = readyState;
	}

	public void OnClickReadyToggle(bool value) {
		if (value) {
			SendReadyToBeginMessage ();
		} else {
			SendNotReadyToBeginMessage ();
		}
	}
}
