using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarBehavior : NetworkBehaviour {

	public GameObject gameController;
	public AltarLightBehavior[] altarLights;

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
    	}
    }

	void OnReceiveItem() {
		if (!isServer) 
			return;

		numCiders++;
		Debug.Log("numCiders: " + numCiders);

		uiController.RpcShowWalkInstruction();
	}

	void OnLevelUp() {
		if (!isServer) 
			return;

		uiController.addScore(numCiders * numCiders);

		numCiders = 0;
		level++;

		uiController.RpcShowBringCiderInstruction();
	}
}
