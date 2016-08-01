using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CiderManager : NetworkBehaviour {

	public GameObject breweryPrefab;
	public GameObject ciderPrefab;
    public int numBreweries;
	public AltarBehavior altar;

	private List<ItemSource> ciderSources;

	void Start() {
		G.get().ciderManager = this;
	}

	public override void OnStartServer()
    {
		ciderSources = new List<ItemSource>();

    	Texture breweryTexture = breweryPrefab.GetComponent<SpriteRenderer>().sprite.texture;
		int breweryWidth = breweryTexture.width;
		int breweryHeight = breweryTexture.height;

		for (int i=0; i < numBreweries; i++)
        {
            Vector3 pos;
			int attempts = 0;
            do {
	            pos = new Vector3(
					randPosNeg(Random.Range(G.HALF_SCREEN_WIDTH, G.HALF_WORLD_WIDTH - breweryWidth)),
					randPosNeg(Random.Range(G.HALF_SCREEN_HEIGHT, G.HALF_WORLD_HEIGHT - breweryHeight)),
					0.0f);

				// Debug.Log("brewery pos: " + pos + " bw " + breweryWidth + " bh "+ breweryHeight);
			} while (collidesWithPrevious(pos, breweryWidth, breweryHeight)
				&& attempts++ < G.get().MAX_SPAWN_ATTEMPTS);
			if (attempts >= G.get().MAX_SPAWN_ATTEMPTS) {
				// Debug.Log("Giving up spawning more brewerys after " + attempts + " attempts");
				return;
			}

            var rotation = Quaternion.identity; //Euler( Random.Range(0,180), Random.Range(0,180), Random.Range(0,180));

			var brewery = (GameObject)Instantiate(breweryPrefab, pos, rotation);
			// brewery.transform.parent = world.transform;
			NetworkServer.Spawn(brewery);
			ciderSources.Add(brewery.GetComponent<BreweryBehaviour>());
        }
        ciderSources.Add(altar);
    }

    public bool HasCider() {
		foreach (ItemSource source in ciderSources) {
			if (source.HasItem()) {
				return true;
			}
		}
		return false;
    }

    public ItemSource GetNearestCiderPos(Vector3 pos) {
    	float nearestSqrDistance = float.MaxValue;
    	ItemSource nearestCiderSource = null;
		foreach (ItemSource source in ciderSources) {
			if (source.HasItem()) {
				float sqrDistance = (source.GetItemPosition() - pos).sqrMagnitude;
				if (sqrDistance < nearestSqrDistance) {
					nearestCiderSource = source;
					nearestSqrDistance = sqrDistance;
				}
			}
		}
		return nearestCiderSource;
    }

    private float randPosNeg(float value) {
		return (Random.value < 0.5) ? -value : value;
    }

	private bool collidesWithPrevious(Vector3 newPos, int w, int h) {
		foreach (ItemSource source in ciderSources) {
			Vector3 delta = source.GetItemPosition() - newPos;
			if (Mathf.Abs(delta.x) < w && Mathf.Abs(delta.y) < h) {
				// Debug.Log("Collides with another brewery!" + prevPos + " "+ newPos + " "+ delta);
				return true;
			}
		}
		return false;
	}
}
