using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CiderManager : NetworkBehaviour {

	public GameObject world;
	public GameObject breweryPrefab;
	public GameObject ciderPrefab;
    public int numBreweries;

	public override void OnStartServer()
    {
		var prevPositions = new List<Vector3>();

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

				Debug.Log("brewery pos: " + pos + " bw " + breweryWidth + " bh "+ breweryHeight);
			} while (collidesWithPrevious(prevPositions, pos, breweryWidth, breweryHeight)
				&& attempts++ < G.get().MAX_SPAWN_ATTEMPTS);
			if (attempts >= G.get().MAX_SPAWN_ATTEMPTS) {
				Debug.Log("Giving up spawning more brewerys after " + attempts + " attempts");
				return;
			}

            var rotation = Quaternion.identity; //Euler( Random.Range(0,180), Random.Range(0,180), Random.Range(0,180));

			var brewery = (GameObject)Instantiate(breweryPrefab, pos, rotation);
			brewery.transform.parent = world.transform;
			NetworkServer.Spawn(brewery);
			prevPositions.Add(pos);
        }
    }

    private float randPosNeg(float value) {
		return (Random.value < 0.5) ? -value : value;
    }

	private bool collidesWithPrevious(List<Vector3> prevPositions, Vector3 newPos, int w, int h) {
		foreach (Vector3 prevPos in prevPositions) {
			Vector3 delta = prevPos - newPos;
			if (Mathf.Abs(delta.x) < w && Mathf.Abs(delta.y) < h) {
				Debug.Log("Collides with another brewery!" + prevPos + " "+ newPos + " "+ delta);
				return true;
			}
		}
		return false;
	}
}
