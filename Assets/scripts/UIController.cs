using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class UIController : NetworkBehaviour {

	public GameObject bringCiderInstruction;
	public GameObject walkInstruction;
	public Text scoreText;
	public Text timerText;

	private float timeElapsed;
	
	// Update is called once per frame
	void Update () {
		float timeLeft = G.get().GOD_ANGRY_DURATION - timeElapsed;
		timerText.text = timeLeft.ToString("%.1f");
	}


	[ClientRpc]
	public void RpcShowBringCiderInstruction() {
		bringCiderInstruction.SetActive(true);
		walkInstruction.SetActive(false);
	}

	[ClientRpc]
	public void RpcShowWalkInstruction() {
		bringCiderInstruction.SetActive(false);
		walkInstruction.SetActive(true);
	}

	[ClientRpc]
	public void RpcHideInstructions() {
		bringCiderInstruction.SetActive(false);
		walkInstruction.SetActive(false);
	}

	[ClientRpc]
	public void RpcUpdateScore(int score) {
		scoreText.text = score.ToString();
	}
}
