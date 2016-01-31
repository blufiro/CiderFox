using UnityEngine;
using System.Collections;

public class ArrowBehaviour : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D collision) {
		var hit = collision.gameObject;
		var hitEnemy = hit.GetComponent<EnemyBehaviour>();
		if (hitEnemy != null) {
			hitEnemy.TakeDamage(G.get().ARROW_DAMAGE);
			Destroy(gameObject);
		}
	}
}
