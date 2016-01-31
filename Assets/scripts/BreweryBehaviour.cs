using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BreweryBehaviour : NetworkBehaviour {

	public GameObject ciderPrefab;

	private IEnumerator infiniteProduceCoroutine;

	public override void OnStartServer()
    {
		infiniteProduceCoroutine = ProduceCider();
		StartCoroutine(infiniteProduceCoroutine);
    }

    void OnDestroy() {
		StopCoroutine(infiniteProduceCoroutine);
    }

    IEnumerator ProduceCider() {
    	// Should never be called on client
    	if (!isServer) {
    		yield return null;
    	} else {
	    	while (true) {
	    		
				Vector3 spawnPos = transform.position;
				GameObject gameObject = (GameObject) Instantiate(ciderPrefab, spawnPos, Quaternion.identity);
				NetworkServer.Spawn(gameObject);

				yield return new WaitForSeconds(G.get().CIDER_PRODUCE_DELAY);
			}
		}
    }
}
