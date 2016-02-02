using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BreweryBehaviour : NetworkBehaviour {

	public GameObject ciderPrefab;

	private bool prevSpawnedCiderExists;
	private float produceTimeElapsed;
	private Vector3 produceSpawnPosition;

	public AudioClip sfx_cider_brewery;

	public override void OnStartServer()
    {
		produceTimeElapsed = 0;
		produceSpawnPosition = gameObject.transform.FindChild("CiderSpawnPoint").position;
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

		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_cider_brewery;
		audio.Play();
    }
}
