using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : NetworkBehaviour {

	public GameObject bringCiderInstruction;
	public GameObject walkInstruction;
	public Text scoreText;
	public GameObject angryBar;
	public GameObject gameOver;
	public Text gameOverScoreText;
	public NetworkManager networkManager;
	public GameObject enemyPrefab;
	public GameObject ciderPrefab;
	public Wave[] waves;

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
	private int currentWaveIndex;
	private float currentWaveMinute;

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
		currentWaveIndex = 0;
		currentWaveMinute = getWaveMinute();
#if DEBUG
		// verify that waves are sequential in timing.
		float previousMinute = 0.0f;
		foreach (Wave wave in waves) {
			if (wave.numEnemies <= 0) {
				throw new UnityException(
					"Waves cannot have <0 num enemies: " + wave.numEnemies);
			}
			if (wave.minute < previousMinute) {
				throw new UnityException(
					"Waves are not ordered in sequential time. Check wave minute:" + wave.minute);
			}
			previousMinute = wave.minute;
		}
#endif
	}

	public override void OnStartServer()
    {
		ResetScore();
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
			Wave w = new Wave();
			w.numEnemies = 3;
			w.minute = Time.timeSinceLevelLoad / 60.0f;
			w.enemySpeedMultiplier = 1.0f;
			SpawnEnemies(w);
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

		if (currentWaveMinute * 60.0f < serverTimeElapsed
			&& currentWaveIndex < waves.Length) {
			Debug.Log(currentWaveIndex +" "+currentWaveMinute);
			Wave currentWave = waves[currentWaveIndex];
			SpawnEnemies(currentWave);
			currentWaveIndex++;
			currentWaveMinute = getWaveMinute();
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

	public void AddScore(int points) {
		if (!isServer)
			return;
		score += points;
	}

	public void OnGameOver(int finalScore) {
		if (!isServer)
			return;
		RpcGameOver(finalScore);
	}

	public void OnLeaveGame() {
		if (isServer) {
			networkManager.StopHost();
			networkManager.StopMatchMaker();
		} else {
			networkManager.StopClient();
			networkManager.StopMatchMaker();
		}
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

	private void ResetScore() {
		score = 0;
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

	private void SpawnEnemies(Wave wave) {
		for (int i=0; i < wave.numEnemies; i++) {
        	switch (wave.spawnLocation) {
        		case Wave.SpawnLocation.SCATTERED:
        		break;
				case Wave.SpawnLocation.TOP_RIGHT:
        		break;
				case Wave.SpawnLocation.TOP_LEFT:
        		break;
				case Wave.SpawnLocation.BOTTOM_RIGHT:
        		break;
				case Wave.SpawnLocation.BOTTOM_LEFT:
        		break;
	        }
			var pos = G.RandCircle(G.SAFE_SPAWN_RADIUS) + G.RandVec2(G.SPAWN_RAND_EXTRA_RADIUS);
            var rotation = Quaternion.identity; //Euler( Random.Range(0,180), Random.Range(0,180), Random.Range(0,180));

            var enemy = (GameObject)Instantiate(enemyPrefab, pos, rotation);
            enemy.GetComponent<EnemyBehaviour>().speedMultiplier = wave.enemySpeedMultiplier;
			// enemy.transform.parent = world.transform;
            NetworkServer.Spawn(enemy);
            // TODO store references to spawned enemies
        }
	}

	private float getWaveMinute() {
		if (currentWaveIndex < waves.Length) {
			return waves[currentWaveIndex].minute;
		} else {
			return float.MaxValue;
		}
	}
}
