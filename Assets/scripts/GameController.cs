using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class GameController : NetworkBehaviour {

	public GameObject bringCiderInstruction;
	public GameObject walkInstruction;
	public Text scoreText;
	public GameObject angryBar;
	public GameObject gameOver;
	public Text gameOverScoreText;
	public NetworkManager networkManager;

	[SyncVar(hook="ScoreUpdated")]
	private int score;
	[SyncVar(hook="TimeUpdated")]
	private float serverTimeElapsed;

	private RectTransform angryBarRectTransform;
	private Vector2 minBarDim;
	private Vector2 originalBarDim;
	private float clientTimeElapsed;
	private bool isGameOver;

	void Start() {
		angryBarRectTransform = angryBar.GetComponent<RectTransform>();
		minBarDim = new Vector2(0, angryBarRectTransform.rect.height);
		originalBarDim = new Vector2(angryBarRectTransform.rect.width, angryBarRectTransform.rect.height);
		scoreText.text = "0";
	}

	public override void OnStartServer()
    {
		resetScore();
    }

	// Update is called once per frame
	void Update () {
		// timeElapsed is from the server.
		clientTimeElapsed += Time.deltaTime;
		float barRatio = Mathf.Clamp01(1 - (clientTimeElapsed / G.get().GOD_ANGRY_DURATION));
		angryBarRectTransform.sizeDelta = Vector2.Lerp(minBarDim, originalBarDim, barRatio);

		if (Input.GetKeyUp(KeyCode.Escape)) {
			OnGameOver(0);
		}

		if (!isServer) 
			return;

		if (isGameOver)
			return;	

		serverTimeElapsed += Time.deltaTime;
		if (serverTimeElapsed >= G.get().GOD_ANGRY_DURATION) {
			gameObject.SendMessage("OnGameOver", score);
			serverTimeElapsed = 0;
		}
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
	public void RpcGameOver(int finalScore) {
		isGameOver = true;
		gameOverScoreText.text = finalScore.ToString();
		gameOver.SetActive(true);
		clientTimeElapsed = G.get().GOD_ANGRY_DURATION;
	}

	[Command]
	public void CmdDisconnect() {
		networkManager.StopHost();
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

	public void OnGameOver(int finalScore) {
		if (!isServer)
			return;
		RpcGameOver(finalScore);
	}

	private void ScoreUpdated(int score) {
		scoreText.text = score.ToString();
	}

	private void TimeUpdated(float newTimeElapsed) {
		if (!isServer) {
			Debug.Log("clientTimeElapsed: "+ clientTimeElapsed+" newTimeElapsed: "+ newTimeElapsed);
			if (Mathf.Abs(clientTimeElapsed - newTimeElapsed) > G.get().SNAP_TIME_THRESHOLD) {
				clientTimeElapsed = newTimeElapsed;
			}
		}
	}
}
