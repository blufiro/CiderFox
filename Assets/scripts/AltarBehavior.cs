using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarBehavior : NetworkBehaviour, ItemSource {

	public AltarLightBehavior[] altarLights;
	public Animator animator;

	public AudioClip sfx_cider_altar;
	public AudioClip sfx_altar_score;
	public AudioClip sfx_altar_fail;

	int numCiders = 0;
	int numRocketsSent = 0;

	public bool HasItem() {
    	return numCiders > 0;
    }

	public Vector3 GetItemPosition() {
    	return transform.position;
    }

	public void OnStealItem() {
		if (!isServer)
			return;
		numCiders--;
		if (numCiders < 0) {
			throw new UnityException("Num ciders should never be less than 0");
		}
		Debug.Log("OnStealItem numCiders: " + numCiders);
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

		G.get().gameController.RpcShowWalkInstruction();
	}

	void OnSendRocket() {
		if (!isServer) 
			return;
		if (numCiders == 0) {
			RpcOnFailRocket();
			return;
		}
    			
		G.get().gameController.AddScore(numCiders * numCiders);

		numCiders = 0;
		numRocketsSent++;
		RpcOnSendRocket ();

		G.get().gameController.RpcShowBringCiderInstruction();
	}

	[ClientRpc]
	void RpcCiderReceived() {
		G.get().gameController.PlayAudio(sfx_cider_altar);
		animator.SetTrigger("ReceiveCider");
	}

	[ClientRpc]
	void RpcOnSendRocket() {
		G.get().gameController.PlayAudio(sfx_altar_score);
	}

	[ClientRpc]
	void RpcOnFailRocket() {
		G.get().gameController.PlayAudio(sfx_altar_fail);
	}
}
