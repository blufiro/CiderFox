using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class UIController : NetworkBehaviour {

	public GameObject bringCiderInstruction;
	public GameObject walkInstruction;
	public Text scoreText;
	public GameObject angryBar;

	[SyncVar(hook="ScoreUpdated")]
	private int score;
	private float timeElapsed;
	private RectTransform angryBarRectTransform;
	private float originalBarWidth;

	void Start() {
		angryBarRectTransform = angryBar.GetComponent<RectTransform>();
		originalBarWidth = angryBarRectTransform.rect.width;
		scoreText.text = "0";
	}

	public override void OnStartServer()
    {
		resetScore();
    }

	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		float timeLeft = G.get().GOD_ANGRY_DURATION - timeElapsed;
		if (timeLeft < 0) {
			timeLeft = 0;
			if (isServer) {
				gameObject.SendMessage("OnAngryTimeUp");
			}
			timeElapsed = 0;
		}
		angryBarRectTransform.sizeDelta = new Vector2(
			timeLeft / G.get().GOD_ANGRY_DURATION * originalBarWidth,
			angryBarRectTransform.rect.height);
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

	public void addScore(int points) {
		if (!isServer)
			return;
		score += points;
	}

	public void resetScore() {
		if (!isServer)
			return;
		score = 0;
	}

	private void ScoreUpdated(int score) {
		scoreText.text = score.ToString();
	}
}
