using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArrowBehaviour : NetworkBehaviour {

	// Unity doesn't send Quarternion with NetworkSpawn, so we have to use a SyncVar
	[SyncVar]
	public Quaternion initial_direction;

	public override void OnStartClient() {
		this.transform.rotation = initial_direction;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		var hit = collider.gameObject;
		var hitEnemy = hit.GetComponent<EnemyBehaviour>();
		if (hitEnemy != null) {
			hitEnemy.CmdTakeDamage(G.get().ARROW_DAMAGE);
			Destroy(gameObject);
		}
	}
}
