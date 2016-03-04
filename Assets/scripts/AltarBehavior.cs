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
		RpcCiderReceived ();
		Debug.Log("numCiders: " + numCiders);

		gameController.RpcShowWalkInstruction();
	}

	void OnLevelUp() {
		if (!isServer) 
			return;

		gameController.addScore(numCiders * numCiders);

		numCiders = 0;
		level++;
		RpcOnLevelUp ();

		gameController.RpcShowBringCiderInstruction();
	}

	[ClientRpc]
	void RpcCiderReceived() {
		gameController.PlayAudio(sfx_cider_altar);
	}

	[ClientRpc]
	void RpcOnLevelUp() {
		gameController.PlayAudio(sfx_altar_score);
	}
}
