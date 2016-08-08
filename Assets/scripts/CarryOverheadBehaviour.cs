using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CarryOverheadBehaviour : NetworkBehaviour {

	[SyncVar]
	private ItemType carryingItemType;

	private GameObject m_carryingItem;

	// Returns true if taken, false otherwise.
	public bool TakeItem(ItemBehaviour itemBehavior) {
		if (!isServer)
			return false;

		if (IsCarryingItem())
			return false;

		// start carrying the item;
		float spriteOffset = G.get().CARRY_OVERHEAD_OFFSET_Y + 
			gameObject.GetComponent<Collider2D>().bounds.size.y / 2.0f;
		m_carryingItem = (GameObject) Instantiate(itemBehavior.itemIconPrefab);
		NetworkServer.Spawn(m_carryingItem);
		RpcCarryingItemInit(m_carryingItem, this.gameObject, spriteOffset);

		carryingItemType = itemBehavior.itemType;

		return true;
	}

	public bool IsCarryingItem() {
		return carryingItemType != ItemType.NONE;
	}

	public void RemoveCarriedItem() {
		if (!isServer)
			return;

		carryingItemType = ItemType.NONE;
		Destroy(m_carryingItem);
	}

	public void DropCarriedItem() {
		if (!isServer)
			return;

		if (!IsCarryingItem())
			return;

		switch (carryingItemType) {
		  case ItemType.CIDER: {
			var newItem = (GameObject)Instantiate(G.get().gameController.ciderPrefab);
			newItem.transform.position = transform.position;
			NetworkServer.Spawn(newItem);
		  } break;
		}

		RemoveCarriedItem();
	}

	[ClientRpc]
	void RpcCarryingItemInit(GameObject carryingItem, GameObject parent, float spriteOffset) {
		Debug.Log ("carryingItem:" + carryingItem);
		carryingItem.transform.parent = parent.transform;
		carryingItem.transform.localPosition = new Vector3(0.0f, spriteOffset, 0.0f);
	}
}
