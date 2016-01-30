using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
            return;

		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");
		transform.Translate(x, y, 0);
	}

	public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().material.color = Color.red;
    }
}
