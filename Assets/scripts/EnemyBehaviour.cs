using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyBehaviour : NetworkBehaviour {

	[SyncVar]
	public int health;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnStartLocalPlayer()
    {
        health = 1;
    }

	public void TakeDamage(int damage) {
		if (!isServer)
			return;

		health -= damage;
		Debug.Log("hit enemy");
		if (health <= 0) {
			Destroy(gameObject);
		}
	}
}
