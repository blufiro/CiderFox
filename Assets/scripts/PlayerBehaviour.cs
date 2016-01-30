using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	public GameObject arrowPrefab;

	private Direction facing;

	// Use this for initialization
	void Start () {
		facing = Direction.DOWN;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
            return;

		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");
		transform.Translate(x, y, 0);

		Direction newFacing = Direction.get(new Vector2(x,y));
		if (newFacing != Direction.NONE) {
			facing = newFacing;
		}

		if (Input.GetKeyDown(KeyCode.Space))
        {
        	// Called from the client but invoked on the server.
            CmdFire();
        }
	}

	public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().material.color = Color.red;
    }

    [Command]
	void CmdFire()
    {
		// create the arrow object locally
        var arrow = (GameObject)Instantiate(
            arrowPrefab,
            transform.position - facing.toVector3(),
            Quaternion.FromToRotation(Direction.RIGHT.toVector3(), facing.toVector3()));

		// make the arrow move away in front of the player
		arrow.GetComponent<Rigidbody2D>().velocity = facing.toVector2() * G.get().ARROW_SPEED;

		// spawn the arrow on the clients
		NetworkServer.Spawn(arrow);
        
		// make arrow disappear after 2 seconds
		Destroy(arrow, G.get().ARROW_LIFE);
    }
}
