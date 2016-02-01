using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DebugStart : MonoBehaviour {

	public bool isDebug;

	// Use this for initialization
	void Start () {
		StartCoroutine(DebugStartGame());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator DebugStartGame() {
		yield return new WaitForSeconds(1);

		if (isDebug) {
			gameObject.GetComponent<NetworkManager>().StartHost();
			yield return new WaitForSeconds(1);
		}
	}
}
