using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CiderManager : NetworkBehaviour {

	public GameObject world;
	public GameObject breweryPrefab;
	public GameObject ciderPrefab;
    public int numBreweries;

	public override void OnStartServer()
    {
    	Texture breweryTexture = breweryPrefab.GetComponent<SpriteRenderer>().sprite.texture;
		int breweryWidth = breweryTexture.width;
		int breweryHeight = breweryTexture.height;

		for (int i=0; i < numBreweries; i++)
        {
            var pos = new Vector3(
				Random.Range(-G.HALF_WORLD_WIDTH + breweryWidth, G.HALF_WORLD_WIDTH - breweryWidth),
				Random.Range(-G.HALF_WORLD_HEIGHT + breweryHeight, G.HALF_WORLD_HEIGHT - breweryHeight),
				0.0f);

            var rotation = Quaternion.identity; //Euler( Random.Range(0,180), Random.Range(0,180), Random.Range(0,180));

			var brewery = (GameObject)Instantiate(breweryPrefab, pos, rotation);
			brewery.transform.parent = world.transform;
			NetworkServer.Spawn(brewery);
        }
    }
}
