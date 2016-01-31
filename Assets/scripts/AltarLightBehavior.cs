using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarLightBehavior : NetworkBehaviour {

	public Sprite lightOn;
	public Sprite lightOff;

	[SyncVar(hook="LightChanged")]
	private bool isOn;

	[Client]
	private void LightChanged(bool value) {
		GetComponent<SpriteRenderer>().sprite = (value) ? lightOn : lightOff;
	}

	public void SwitchOn() {
		if (!isServer) {
			return;
		}

		isOn = true;

		StartCoroutine("SwitchOff");
	}

	IEnumerator SwitchOff() {
		float delay = G.get().LIGHT_OFF_DELAY;
		yield return new WaitForSeconds(delay);

		isOn = false;
	}

	void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log("Collide");
		var hitPlayer = collision.gameObject.GetComponent<PlayerBehaviour>();
		if (hitPlayer != null) {
			Debug.Log("Collide with player");
			SwitchOn();
		}
	}
}
