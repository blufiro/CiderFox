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
	public GameObject enemyPrefab;

	[SyncVar(hook="ScoreUpdated")]
	private int score;
	[SyncVar(hook="TimeUpdated")]
	private float serverTimeElapsed;

	private RectTransform angryBarRectTransform;
	private Vector2 minBarDim;
	private Vector2 originalBarDim;
	private float clientTimeElapsed;
	private bool isGameOver;
	private int numThiefRanAway;
	private AudioSource[] audioSources;
	private int nextAudioSource;

	void Start() {
		G.get().gameController = this;
		angryBarRectTransform = angryBar.GetComponent<RectTransform>();
		minBarDim = new Vector2(0, angryBarRectTransform.rect.height);
		originalBarDim = new Vector2(angryBarRectTransform.rect.width, angryBarRectTransform.rect.height);
		scoreText.text = "0";
		audioSources = new AudioSource[G.MAX_AUDIO_SOURCES];
		nextAudioSource = 0;
		for (int i = 0; i < G.MAX_AUDIO_SOURCES; i++) {
			audioSources [i] = gameObject.AddComponent<AudioSource>();
		}
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

		if (Input.GetKeyUp(KeyCode.E)) {
			RpcSpawnEnemies(3);
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

	[ClientRpc]
	void RpcSpawnEnemies(int numEnemies) {
		for (int i=0; i < numEnemies; i++)
        {
        	float randAngle = Random.Range(0.0f, 2 * Mathf.PI);
            var pos = new Vector3(
				G.SAFE_SPAWN_RADIUS * Mathf.Cos(randAngle) + Random.value * G.SPAWN_RAND_EXTRA_RADIUS,
				G.SAFE_SPAWN_RADIUS * Mathf.Sin(randAngle) + Random.value * G.SPAWN_RAND_EXTRA_RADIUS,
				0.0f);

            var rotation = Quaternion.identity; //Euler( Random.Range(0,180), Random.Range(0,180), Random.Range(0,180));

            var enemy = (GameObject)Instantiate(enemyPrefab, pos, rotation);
			// enemy.transform.parent = world.transform;
            NetworkServer.Spawn(enemy);
            // TODO store references to spawned enemies
        }
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

	public void PlayAudio(AudioClip clip) {
		int freeIndex = -1;
		for (int i=nextAudioSource; i< (nextAudioSource+G.MAX_AUDIO_SOURCES); i++) {
			if (!audioSources [i % G.MAX_AUDIO_SOURCES].isPlaying) {
				freeIndex = i;
				break;
			}
		}
		if (freeIndex == -1) {
			throw new UnityException ("Not enough audio sources or all occupied");
		}
		audioSources [freeIndex].clip = clip;
		audioSources [freeIndex].Play ();
		nextAudioSource = (freeIndex + 1) % G.MAX_AUDIO_SOURCES;
	}

	public void OnThiefRanAway() {
		numThiefRanAway++;
	}

	private void ScoreUpdated(int score) {
		scoreText.text = score.ToString();
	}

	private void TimeUpdated(float newTimeElapsed) {
		if (!isServer) {
			if (Mathf.Abs(clientTimeElapsed - newTimeElapsed) > G.get().SNAP_TIME_THRESHOLD) {
				clientTimeElapsed = newTimeElapsed;
			}
		}
	}
}
