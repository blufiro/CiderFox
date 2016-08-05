using UnityEngine;
using System.Collections;

public class ArrowBehaviour : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider) {
		var hit = collider.gameObject;
		Debug.Log("Arrow hit collision" + hit.name);
		var hitEnemy = hit.GetComponent<EnemyBehaviour>();
		if (hitEnemy != null) {
			hitEnemy.CmdTakeDamage(G.get().ARROW_DAMAGE);
			Destroy(gameObject);
		}
	}
}
