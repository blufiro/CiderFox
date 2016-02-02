using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarBehavior : NetworkBehaviour {

	public GameController gameController;
	public AltarLightBehavior[] altarLights;

	public AudioClip sfx_cider_altar;
	public AudioClip sfx_altar_score;

	int numCiders = 0;
	int level = 1;

    void Update() {
    	if (!isServer)
    		return;

    	bool allLightsOn = true;
    	foreach (AltarLightBehavior altarLight in altarLights) {
    		if (!altarLight.isOn()) {
    			allLightsOn = false;
    			break;
    		}
    	}

    	if (allLightsOn) {
    		OnLevelUp();
    	}
    }

	void OnReceiveItem() {
		if (!isServer) 
			return;

		numCiders++;
		Debug.Log("numCiders: " + numCiders);

		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_cider_altar;
		audio.Play();

		gameController.RpcShowWalkInstruction();
	}

	void OnLevelUp() {
		if (!isServer) 
			return;

		gameController.addScore(numCiders * numCiders);

		numCiders = 0;
		level++;

		gameController.RpcShowBringCiderInstruction();

		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_altar_score;
		audio.Play();
	}
}
