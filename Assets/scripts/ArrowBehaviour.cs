using UnityEngine;
using System.Collections;

public class ArrowBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {
		var hit = collision.gameObject;
		var hitEnemy = hit.GetComponent<EnemyBehaviour>();
		if (hitEnemy != null) {
			hitEnemy.TakeDamage(G.get().ARROW_DAMAGE);
			Destroy(gameObject);
		}
	}
}
