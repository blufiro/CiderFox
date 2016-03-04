using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AltarLightBehavior : NetworkBehaviour {

	public Sprite lightOn;
	public Sprite lightOff;

	public AudioClip sfx_light_on;
	public AudioClip sfx_light_off;

	private bool m_isOn;

	public void SwitchOn() {
		if (!isServer) {
			return;
		}

		RpcLightChanged(true);

		StartCoroutine("SwitchOff");
	}

	IEnumerator SwitchOff() {
		yield return new WaitForSeconds(G.get().LIGHT_OFF_DELAY);

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
		m_isOn = value;
		GetComponent<SpriteRenderer>().sprite = (value) ? lightOn : lightOff;

		G.get().gameController.PlayAudio((value) ? sfx_light_on : sfx_light_off);
	}

	public bool isOn() {
		return m_isOn;
	}
}
