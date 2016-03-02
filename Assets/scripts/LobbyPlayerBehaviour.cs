using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class LobbyPlayerBehaviour : NetworkLobbyPlayer {

	public GameObject lobbyUITemplate;

	private GameObject lobbyUI;
	private Toggle readyToggle;

	public override void OnClientEnterLobby() {
		base.OnClientEnterLobby ();
		Debug.Log ("lobbyplayer.OnClientEnterLobby");
		GameObject canvas = GameObject.Find ("Canvas");

		clearUI ();
		lobbyUI = (GameObject)Instantiate (lobbyUITemplate);
		readyToggle = lobbyUI.transform.GetComponentInChildren<Toggle> ();
		lobbyUI.transform.SetParent(canvas.transform, false);
		float width = lobbyUI.GetComponent<RectTransform> ().sizeDelta.x;
		lobbyUI.transform.localPosition = (new Vector3 ( (this.slot == 0) ? -width : width, 0));

		readyToggle.interactable = false;
	}

	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		readyToggle.interactable = true;
		readyToggle.onValueChanged.AddListener(OnClickReadyToggle);
	}

	public override void OnClientExitLobby() {
		base.OnClientExitLobby ();
		Debug.Log ("lobbyplayer.OnClientExitLobby");
		clearUI ();
	}

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

	private void clearUI() {
		if (lobbyUI != null) {
			Destroy (lobbyUI);
			lobbyUI = null;
			readyToggle = null;
		}
	}
}
