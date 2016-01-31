using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UIController : NetworkBehaviour {

	public GameObject bringCiderInstruction;
	public GameObject walkInstruction;
	
	// Update is called once per frame
	void Update () {
	
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
}
