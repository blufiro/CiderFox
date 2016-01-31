using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarBehavior : NetworkBehaviour {

	int numCiders = 0;

	void OnReceiveItem() {
		numCiders++;
		Debug.Log("numCiders: " + numCiders);
	}
}
