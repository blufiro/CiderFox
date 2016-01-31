using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ItemBehaviour : NetworkBehaviour {

	public GameObject itemIconPrefab;

	void OnTriggerEnter2D(Collider2D collision) {
		if (!isServer)
			return;

		var hitBearer = collision.gameObject.GetComponent<CarryOverheadBehaviour>();
		if (hitBearer != null) {
			if (hitBearer.TakeItem(this)) {
				Destroy(gameObject);
			}
		}
	}
}
