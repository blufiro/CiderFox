using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarLightBehavior : NetworkBehaviour {

	public Sprite lightOn;
	public Sprite lightOff;

	public void SwitchOn() {
		if (!isServer) {
			return;
		}

		RpcLightChanged(true);

		StartCoroutine("SwitchOff");
	}

	IEnumerator SwitchOff() {
		float delay = G.get().LIGHT_OFF_DELAY;
		yield return new WaitForSeconds(delay);

		RpcLightChanged(false);
	}

	void OnTriggerEnter2D(Collider2D collision) {
		var hitPlayer = collision.gameObject.GetComponent<PlayerBehaviour>();
		if (hitPlayer != null) {
			SwitchOn();
		}
	}

	[ClientRpc]
	void RpcLightChanged(bool value) {
		GetComponent<SpriteRenderer>().sprite = (value) ? lightOn : lightOff;
	}
}
