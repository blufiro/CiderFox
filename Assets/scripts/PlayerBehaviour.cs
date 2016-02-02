using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	public GameObject arrowPrefab;
	public GameObject target;

	public AudioClip sfx_player_target;


	[SyncVar(hook="FacingChanged")]
	private int networkFacing;
	private Direction facing;

	[SyncVar]
	private Vector2 destination;

	// private GameObject world;
	private Vector3 tapBegin;
	private bool isAiming;
	private Animator animator;
	private Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		facing = Direction.DOWN;
		// world = GameObject.Find("World");
		// transform.parent = world.transform;
		Input.simulateMouseWithTouches = true;
		rigidBody = GetComponent<Rigidbody2D>();
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
				// newTarget.transform.parent = world.transform;
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
	            // CmdFire();
				isAiming = false;
    		}
        }

        // Move towards destination
		Vector2 currPos = transform.position;
		Vector2 toDestination = destination - currPos;
		float distance = toDestination.sqrMagnitude;
		if (distance > 0) {
			Vector2 moveVec = toDestination;
			if (distance > G.get().PLAYER_MOVE_SPEED * G.get().PLAYER_MOVE_SPEED) {
				toDestination.Normalize();
				moveVec = toDestination * G.get().PLAYER_MOVE_SPEED;
			}
			walk(moveVec);
		}
	}

	public override void OnStartServer() {
		// animator needs to exist before networkFacing can take effect.
		animator = GetComponent<Animator>();
		networkFacing = facing.toInt();
	}

	public override void OnStartClient() {
		// animator needs to exist before networkFacing can take effect.
		animator = GetComponent<Animator>();
	}

	public override void OnStartLocalPlayer() {
		destination = transform.position;
		Camera.main.transform.position = new Vector3(
			transform.position.x,
			transform.position.y,
			Camera.main.transform.position.z);
		var smoothCam = Camera.main.GetComponent<SmoothCamera2D>();
		smoothCam.target = this.transform;
		smoothCam.bounds = new Rect(
			-G.HALF_WORLD_WIDTH + G.HALF_SCREEN_WIDTH,
			-G.HALF_WORLD_HEIGHT + G.HALF_SCREEN_HEIGHT,
			G.WORLD_WIDTH - G.SCREEN_WIDTH,
			G.WORLD_HEIGHT - G.SCREEN_HEIGHT);
    }

//    [Command]
//	void CmdFire()
//    {
//		Debug.Log("Fire");
//		// create the arrow object locally
//        var arrow = (GameObject)Instantiate(
//            arrowPrefab,
//			transform.position - facing.toVector3(),
//			Quaternion.FromToRotation(Direction.RIGHT.toVector3(), facing.toVector3()));
//		// arrow.transform.parent = world.transform;
//
//		// make the arrow move away in front of the player
//		arrow.GetComponent<Rigidbody2D>().velocity = facing.toVector2() * G.get().ARROW_SPEED;
//
//		// spawn the arrow on the clients
//		NetworkServer.Spawn(arrow);
//        
//		// make arrow disappear after 2 seconds
//		Destroy(arrow, G.get().ARROW_LIFE);
//    }

    private void walk(Vector2 moveVec) {
		Vector3 newPosition = moveVec;
		rigidBody.MovePosition(transform.position + newPosition);
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
		animator.SetInteger("Direction", networkFacing);

    }

    [Client]
	void FacingChanged(int newFacing) {
		facing = Direction.fromInt(newFacing);
		animator.SetInteger("Direction", newFacing);
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

		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_player_target;
		audio.Play();
	}
}
