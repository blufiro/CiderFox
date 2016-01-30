using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	public GameObject arrowPrefab;
	private GameObject world;
	[SyncVar(hook="FacingChanged")]
	private int networkFacing;
	private Direction facing;

	// Use this for initialization
	void Start () {
		facing = Direction.DOWN;
		world = GameObject.Find("World");
		transform.parent = world.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
            return;
        }

		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");
		transform.Translate(x, y, 0);

		Direction newFacing = Direction.get(new Vector2(x,y));
		if (newFacing.toInt() != Direction.NONE.toInt()
			&& newFacing.toInt() != networkFacing) {
			CmdUpdateFacing(newFacing.toInt());
		}

		if (Input.GetKeyDown(KeyCode.Space))
        {
        	// Called from the client but invoked on the server.
            CmdFire();
        }
	}

	public override void OnStartServer() {
		if (isServer) {
			networkFacing = facing.toInt();
		}
	}

	public override void OnStartLocalPlayer() {
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
		arrow.transform.parent = world.transform;

		// make the arrow move away in front of the player
		arrow.GetComponent<Rigidbody2D>().velocity = facing.toVector2() * G.get().ARROW_SPEED;

		// spawn the arrow on the clients
		NetworkServer.Spawn(arrow);
        
		// make arrow disappear after 2 seconds
		Destroy(arrow, G.get().ARROW_LIFE);
    }

	[Command]
	void CmdUpdateFacing(int newFacing) {
		Debug.Log("update facing newFacing: " + newFacing);
    	networkFacing = newFacing;
    }

    [Client]
	void FacingChanged(int newFacing) {
		facing = Direction.fromInt(newFacing);
	}
}
