using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyBehaviour : NetworkBehaviour {

	[SyncVar]
	public int health;

	public virtual void Update() {
		if (!isServer) {
			return;
		}

		Debug.Log(health);
		if (health <= 0) {
			Debug.Log("health zero!");
			SendMessage("OnDefeat", transform.position, SendMessageOptions.DontRequireReceiver);
			Destroy(this.gameObject);
		}
	}

	[Command]
	public void CmdTakeDamage(int damage) {
		health -= damage;
		Debug.Log("hit enemy with damage: " + damage + " and health left:" + health);
	}
}
