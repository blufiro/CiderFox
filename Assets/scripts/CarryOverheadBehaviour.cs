using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CarryOverheadBehaviour : NetworkBehaviour {

	[SyncVar]
	private bool isCarryingItem;

	private GameObject m_carryingItem;

	// Returns true if taken, false otherwise.
	public bool TakeItem(ItemBehaviour itemBehavior) {
		if (!isServer)
			return false;

		if (isCarryingItem)
			return false;

		// start carrying the item;
		float spriteOffset = G.get().CARRY_OVERHEAD_OFFSET_Y + 
			gameObject.GetComponent<Collider2D>().bounds.size.y / 2.0f;
		m_carryingItem = (GameObject) Instantiate(itemBehavior.itemIconPrefab);
		NetworkServer.Spawn(m_carryingItem);
		RpcCarryingItemInit(m_carryingItem, this.gameObject, spriteOffset);

		isCarryingItem = true;

		return true;
	}

	public bool IsCarryingItem() {
		return isCarryingItem;
	}

	public void RemoveCarriedItem() {
		if (!isServer)
			return;

		isCarryingItem = false;
		Destroy(m_carryingItem);
	}

	[ClientRpc]
	void RpcCarryingItemInit(GameObject carryingItem, GameObject parent, float spriteOffset) {
		Debug.Log ("carryingItem:" + carryingItem);
		carryingItem.transform.parent = parent.transform;
		carryingItem.transform.localPosition = new Vector3(0.0f, spriteOffset, 0.0f);
	}
}
