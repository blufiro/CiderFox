using UnityEngine;

public interface ItemSource {
	bool HasItem();
	Vector3 GetItemPosition();
	void OnStealItem();
}
