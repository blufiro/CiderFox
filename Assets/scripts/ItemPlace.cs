using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ItemPlace : NetworkBehaviour {

	void OnCollisionEnter2D(Collision2D collision) {
		if (!isServer) 
			return;

		var hitItemBearer = collision.gameObject.GetComponent<CarryOverheadBehaviour>();
		if (hitItemBearer != null) {
			if (!hitItemBearer.IsCarryingItem())
				return;
			TakeCarriedItem(hitItemBearer);
		}
	}

	void TakeCarriedItem(CarryOverheadBehaviour itemBearer) {
		if (!isServer) {
			return;
		}
		itemBearer.RemoveCarriedItem();
		// TODO (can send over item id)
		gameObject.SendMessage("OnReceiveItem");
	}
}
