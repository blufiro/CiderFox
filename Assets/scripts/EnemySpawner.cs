using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySpawner : NetworkBehaviour {

	public GameObject world;
	public GameObject enemyPrefab;
    public int numEnemies;

	public override void OnStartServer()
    {
        for (int i=0; i < numEnemies; i++)
        {
            var pos = new Vector3(
				Random.Range(-G.HALF_WORLD_WIDTH, G.HALF_WORLD_WIDTH),
				Random.Range(-G.HALF_WORLD_HEIGHT, G.HALF_WORLD_HEIGHT),
				0.0f);

            var rotation = Quaternion.identity; //Euler( Random.Range(0,180), Random.Range(0,180), Random.Range(0,180));

            var enemy = (GameObject)Instantiate(enemyPrefab, pos, rotation);
			// enemy.transform.parent = world.transform;
            NetworkServer.Spawn(enemy);
        }
    }
}
