using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CarryOverheadBehaviour : NetworkBehaviour {

	[SyncVar]
	private bool isCarryingItem;

	// Returns true if taken, false otherwise.
	public bool TakeItem(ItemBehaviour item) {
		if (!isServer)
			return false;

		if (isCarryingItem)
			return false;

		// start carrying the item;
		float spriteOffset = G.get().CARRY_OVERHEAD_OFFSET_Y + 
			gameObject.GetComponent<SpriteRenderer>().sprite.texture.height / 2.0f;
		GameObject carryingItem = (GameObject) Instantiate(item.itemIconPrefab);
		carryingItem.transform.parent = transform;
		carryingItem.transform.localPosition = new Vector3(0.0f, spriteOffset, 0.0f);
		NetworkServer.Spawn(carryingItem);

		isCarryingItem = true;

		return true;
	}

	public bool HasCarriedItem() {
		return isCarryingItem;
	}

	public bool PutCarriedItem() {
		if (!isServer)
			return false;
		if (!isCarryingItem)
			return false;
		// TODO

		return true;
	}
}
