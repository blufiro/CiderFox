using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ItemBehaviour : NetworkBehaviour {

	public GameObject itemIconPrefab;

	public delegate void TakenAction();
	public event TakenAction OnTaken;

	void OnTriggerEnter2D(Collider2D collision) {
		if (!isServer)
			return;

		var hitBearer = collision.gameObject.GetComponent<CarryOverheadBehaviour>();
		if (hitBearer != null) {
			if (hitBearer.TakeItem(this)) {
				if (OnTaken != null) {
					OnTaken();
				}
				Destroy(gameObject);
			}
		}
	}
}
