using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class LobbyPlayerBehaviour : NetworkLobbyPlayer {

	public Toggle readyToggle;

//	public override void OnStartServer() {
//		Debug.Log ("lobbyplayer.OnStartServer");
//	}

	public override void OnStartClient() {
		Debug.Log ("lobbyplayer.OnStartClient");
		GameObject canvas = GameObject.Find ("Canvas");
		this.transform.SetParent(canvas.transform, false);
		float width = this.GetComponent<RectTransform> ().sizeDelta.x;
		this.transform.Translate (new Vector3 (this.slot * width - width / 2, 0));
	}

	public override void OnStartLocalPlayer() {
		Debug.Log ("lobbyplayer.OnStartLocalPlayer");
		readyToggle.gameObject.SetActive (true);
		readyToggle.onValueChanged.AddListener(OnClickReadyToggle);
	}
//
//	public override void OnClientEnterLobby() {
//	}
//
//	public override void OnClientExitLobby() {
//	}
//
//	public override void OnClientReady() {
//	}

	public void OnClickReadyToggle(bool value) {
		if (value) {
			SendReadyToBeginMessage ();
		} else {
			SendNotReadyToBeginMessage ();
		}
	}
}
