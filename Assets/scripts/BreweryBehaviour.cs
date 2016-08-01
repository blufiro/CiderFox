using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BreweryBehaviour : NetworkBehaviour, ItemSource {

	public GameObject ciderPrefab;
	public AudioClip sfx_cider_brewery;

	private ItemBehaviour prevSpawnedCider;
	private float produceTimeElapsed;
	private Vector3 produceSpawnPosition;

	public override void OnStartServer()
    {
		prevSpawnedCider = null;
		produceTimeElapsed = 0;
		produceSpawnPosition = gameObject.transform.FindChild("CiderSpawnPoint").position;
    }

	public bool HasItem() {
		return prevSpawnedCider != null;
	}

	public Vector3 GetItemPosition() {
		return transform.position;
	}

	public void OnStealItem() {
		if (!isServer)
    		return;

		Debug.Log("BreweryBehavior OnStealItem");
		RpcPlayCiderTakenAudio ();
		// Stealing does not remove Item since collision was not used, so we remove the item manually.
		Debug.Log("ItemBehavior destroy");
		Destroy(prevSpawnedCider.gameObject);
		prevSpawnedCider = null;
	}

    void Update() {
    	if (!isServer)
    		return;

		if (HasItem()) {
    		// do nothing
    	} else {
	    	produceTimeElapsed -= Time.deltaTime;
	    	if (produceTimeElapsed <= 0) {
				GameObject spawned = (GameObject) Instantiate(
					ciderPrefab, produceSpawnPosition, Quaternion.identity);
				var spawnedCider = spawned.GetComponent<ItemBehaviour>();
				spawnedCider.OnTaken += OnCiderTaken;
				NetworkServer.Spawn(spawned);
				produceTimeElapsed = G.get().CIDER_PRODUCE_DELAY;
				prevSpawnedCider = spawnedCider;
			}
		}
    }

    void OnCiderTaken() {
		if (!isServer)
			return;

		prevSpawnedCider = null;
		RpcPlayCiderTakenAudio ();
    }

	[ClientRpc]
	void RpcPlayCiderTakenAudio() {
		G.get().gameController.PlayAudio(sfx_cider_brewery);
	}
}
