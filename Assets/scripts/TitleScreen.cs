using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	public GameObject titleMenu;
	public GameObject lobbyMenu;
	public GameObject creditsMenu;

	public GameObject networkManager;

	void Start() {
		switchState(State.TITLE);
	}

	public void OnClickStart() {
		switchState(State.LOBBY);
	}

	public void OnClickCredits() {
		switchState(State.CREDITS);
	}

	public void OnClickQuit() {
		Application.Quit();
	}

	public void OnClickBackToTitle() {
		switchState(State.TITLE);
	}

	enum State {
		TITLE,
		LOBBY,
		CREDITS,
	}
	private void switchState(State state) {
		titleMenu.SetActive(false);
		lobbyMenu.SetActive(false);
		creditsMenu.SetActive(false);
		networkManager.GetComponent<NetworkManagerHUD>().showGUI = false;
		switch (state) {
			case State.TITLE: titleMenu.SetActive(true); break;
			case State.LOBBY:
				lobbyMenu.SetActive(true);
				networkManager.GetComponent<NetworkManagerHUD>().showGUI = true;
				break;
			case State.CREDITS: creditsMenu.SetActive(true); break;
		}
	}
}
