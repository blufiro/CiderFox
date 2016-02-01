using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class UIController : NetworkBehaviour {

	public GameObject bringCiderInstruction;
	public GameObject walkInstruction;
	public Text scoreText;
	public Text timerText;
	public GameObject angryBar;

	private float timeElapsed;
	private RectTransform angryBarRectTransform;
	private float originalBarWidth;

	void Start() {
		angryBarRectTransform = angryBar.GetComponent<RectTransform>();
		originalBarWidth = angryBarRectTransform.rect.width;
	}

	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		float timeLeft = G.get().GOD_ANGRY_DURATION - timeElapsed;
		timerText.text = timeLeft.ToString("F1");
		angryBarRectTransform.sizeDelta = new Vector2(timeLeft / G.get().GOD_ANGRY_DURATION * 100, 100.0f);
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
