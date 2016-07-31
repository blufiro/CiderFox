using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarBehavior : NetworkBehaviour {

	public GameController gameController;
	public AltarLightBehavior[] altarLights;
	public Animator animator;

	public AudioClip sfx_cider_altar;
	public AudioClip sfx_altar_score;
	public AudioClip sfx_altar_fail;

	int numCiders = 0;
	int numRocketsSent = 0;

	public bool HasCider() {
    	return numCiders > 0;
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
			OnSendRocket();
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

	void OnStealItem() {
		if (!isServer)
			return;
		numCiders--;
		if (numCiders < 0) {
			throw new UnityException("Num ciders should never be less than 0");
		}
		Debug.Log("OnStealItem numCiders: " + numCiders);
	}

	void OnSendRocket() {
		if (!isServer) 
			return;
		if (numCiders == 0) {
			RpcOnFailRocket();
			return;
		}
    			
		gameController.addScore(numCiders * numCiders);

		numCiders = 0;
		numRocketsSent++;
		RpcOnSendRocket ();

		gameController.RpcShowBringCiderInstruction();
	}

	[ClientRpc]
	void RpcCiderReceived() {
		gameController.PlayAudio(sfx_cider_altar);
		animator.SetTrigger("ReceiveCider");
	}

	[ClientRpc]
	void RpcOnSendRocket() {
		gameController.PlayAudio(sfx_altar_score);
	}

	[ClientRpc]
	void RpcOnFailRocket() {
		gameController.PlayAudio(sfx_altar_fail);
	}
}
