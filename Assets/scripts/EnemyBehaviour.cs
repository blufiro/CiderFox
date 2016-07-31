using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyBehaviour : NetworkBehaviour {

	[SyncVar]
	public int health;

	void Update() {
		if (health <= 0) {
			SendMessage("OnDefeat", transform.position, SendMessageOptions.DontRequireReceiver);
			Destroy(this.gameObject);
		}
	}

	public void TakeDamage(int damage) {
		if (!isServer)
			return;

		health -= damage;
		Debug.Log("hit enemy");
	}
}
