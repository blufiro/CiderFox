using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ItemPlace : NetworkBehaviour {

	void OnTriggerEnter2D(Collider2D collision) {
		if (!isServer) 
			return;

		var hitItemBearer = collision.gameObject.GetComponent<CarryOverheadBehaviour>();
		if (hitItemBearer != null) {
			Debug.Log("hit carry overhaeed");
			if (!hitItemBearer.IsCarryingItem())
				return;
			TakeCarriedItem(hitItemBearer);
		}
	}

	void TakeCarriedItem(CarryOverheadBehaviour itemBearer) {
		if (!isServer) {
			return;
		}

		Debug.Log("TakeCarriedItem");
		itemBearer.RemoveCarriedItem();
		// TODO (can send over item id)
		gameObject.SendMessage("OnReceiveItem");
	}
}
