using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarBehavior : NetworkBehaviour {

	public GameObject gameController;
	public AltarLightBehavior[] altarLights;

	public AudioClip sfx_cider_altar;
	public AudioClip sfx_altar_score;

	private UIController uiController;

	int numCiders = 0;
	int level = 1;

	public override void OnStartServer()
    {
		uiController = gameController.GetComponent<UIController>();
    }

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

			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = sfx_altar_score;
			audio.Play();
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

		uiController.RpcShowWalkInstruction();
	}

	void OnLevelUp() {
		if (!isServer) 
			return;

		numCiders = 0;
		level++;

		uiController.RpcShowBringCiderInstruction();
	}
}
