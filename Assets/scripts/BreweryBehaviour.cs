using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BreweryBehaviour : NetworkBehaviour {

	public GameObject ciderPrefab;

	private bool prevSpawnedCiderExists;
	private float produceTimeElapsed;
	private Vector3 produceSpawnPosition;

	public override void OnStartServer()
    {
		produceTimeElapsed = 0;
		float breweryHalfHeight = GetComponent<SpriteRenderer>().sprite.texture.height / 2.0f;
		produceSpawnPosition = transform.position + new Vector3(0, -breweryHalfHeight, 0);
    }

    void Update() {
    	if (!isServer)
    		return;

		if (prevSpawnedCiderExists) {
    		// do nothing
    	} else {
	    	produceTimeElapsed -= Time.deltaTime;
	    	if (produceTimeElapsed <= 0) {
				GameObject spawned = (GameObject) Instantiate(
					ciderPrefab, produceSpawnPosition, Quaternion.identity);
				spawned.GetComponent<ItemBehaviour>().OnTaken += OnCiderTaken;
				NetworkServer.Spawn(spawned);
				produceTimeElapsed = G.get().CIDER_PRODUCE_DELAY;
				prevSpawnedCiderExists = true;
			}
		}
    }

    void OnCiderTaken() {
		prevSpawnedCiderExists = false;
    }
}
