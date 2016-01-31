using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	public GameObject arrowPrefab;
	public GameObject target;

	[SyncVar(hook="FacingChanged")]
	private int networkFacing;
	private Direction facing;

	[SyncVar]
	private Vector2 destination;

	private GameObject world;
	private Vector3 tapBegin;
	private bool isAiming;


	// Use this for initialization
	void Start () {
		facing = Direction.DOWN;
		world = GameObject.Find("World");
		transform.parent = world.transform;
		Input.simulateMouseWithTouches = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
            return;
        }

        // Tap to move
        if (Input.GetMouseButtonDown(0)) {
			Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (GetComponent<Collider2D>().OverlapPoint(touchWorldPos)) {
				tapBegin = Input.mousePosition;
				isAiming = true;
    			CmdStopAndAim();
    		} else {
				isAiming = false;
				GameObject newTarget = Instantiate(target);
				newTarget.transform.parent = world.transform;
				newTarget.transform.position = touchWorldPos;
				Destroy(newTarget, 1.0f);
				CmdMove(touchWorldPos);
    		}
		} else if (Input.GetMouseButton(0)) {
			if (isAiming) {
				Vector2 oppositeFromDragVec = -(Input.mousePosition - tapBegin);
				Direction newFacing = Direction.get(oppositeFromDragVec);
				lazyUpdateFacing(newFacing);
    		}
		} else if (Input.GetMouseButtonUp(0)) {
			if (isAiming) {
				// Called from the client but invoked on the server.
	            CmdFire();
				isAiming = false;
    		}
        }

        // Move towards destination
		Vector2 currPos = transform.position;
		Vector2 toDestination = destination - currPos;
		float distance = toDestination.sqrMagnitude;
		if (distance > 0) {
			Vector2 moveVec = toDestination;
			if (distance > G.get().PLAYER_MOVE_SPEED) {
				toDestination.Normalize();
				moveVec = toDestination * G.get().PLAYER_MOVE_SPEED;
			}
			transform.Translate(moveVec);
		}

//		var x = Input.GetAxis("Horizontal");
//		var y = Input.GetAxis("Vertical");
//		transform.Translate(x, y, 0);
//
//		Direction newFacing = Direction.get(new Vector2(x,y));
//		if (newFacing.toInt() != Direction.NONE.toInt()
//			&& newFacing.toInt() != networkFacing) {
//			CmdUpdateFacing(newFacing.toInt());
//		}
//
//		if (Input.GetKeyDown(KeyCode.Space))
//        {
//        	// Called from the client but invoked on the server.
//            CmdFire();
//        }
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
		Debug.Log("Fire");
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

    private void walk(Vector2 moveVec) {
		Debug.Log("moving... " + moveVec.x + "," + moveVec.y);

		transform.Translate(moveVec);
		Direction newFacing = Direction.get(moveVec);
		lazyUpdateFacing(newFacing);
    }
    private void lazyUpdateFacing(Direction newFacing) {
    	if (newFacing.toInt() != Direction.NONE.toInt()
    		&& networkFacing != newFacing.toInt()) {
    		CmdUpdateFacing(newFacing.toInt());
    	}
    }

	[Command]
	void CmdUpdateFacing(int newFacing) {
		Debug.Log("update facing newFacing: " + newFacing + " from : " + networkFacing);
    	networkFacing = newFacing;
    	facing = Direction.fromInt(networkFacing);
    }

    [Client]
	void FacingChanged(int newFacing) {
		facing = Direction.fromInt(newFacing);
	}

	[Command]
	void CmdStopAndAim() {
		Debug.Log("Stop And Aim");
		destination = transform.position;

	}

	[Command]
	void CmdMove(Vector2 dest) {
		Debug.Log("Move to dest " + dest.x + " "+dest.y);
		destination = dest;
	}
}
