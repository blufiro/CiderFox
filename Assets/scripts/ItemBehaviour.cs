using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ItemBehaviour : NetworkBehaviour {
	public ItemType itemType;
	public GameObject itemIconPrefab;

	public delegate void TakenAction();
	public event TakenAction OnTaken;

	void OnTriggerEnter2D(Collider2D collision) {
		if (!isServer)
			return;

		var hitBearer = collision.gameObject.GetComponent<CarryOverheadBehaviour>();
		Debug.Log("hitBearer " + hitBearer);
		if (hitBearer != null) {
			if (hitBearer.TakeItem(this)) {
				if (OnTaken != null) {
					OnTaken();
				}
				Debug.Log("ItemBehavior destroy");
				Destroy(gameObject);
			}
		}
	}
}
