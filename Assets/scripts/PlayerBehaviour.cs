using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	public GameObject arrowPrefab;
	public GameObject arrowIcon;
	public GameObject target;

	public AudioClip sfx_player_target;

	private Direction facing = Direction.DOWN;
	private string animAction =  "player_idle_";
	[SyncVar(hook="HookFacingChanged")]
	private int networkFacing;
	[SyncVar(hook="HookAnimActionChanged")]
	private string networkAnimAction;
	[SyncVar]
	private Vector2 destination;

	// private GameObject world;
	private Vector3 tapBegin;
	private Vector2 aimVec;
	private bool isAiming;
	private Animator animator;
	private Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		// world = GameObject.Find("World");
		// transform.parent = world.transform;
		Input.simulateMouseWithTouches = true;
		isAiming = false;
		arrowIcon.SetActive(isAiming);
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
				arrowIcon.SetActive(isAiming);
    			stop(transform.position);
    		} else {
				isAiming = false;
				arrowIcon.SetActive(isAiming);
				// Only instantiate for local player
				GameObject newTarget = Instantiate(target);
				// newTarget.transform.parent = world.transform;
				newTarget.transform.position = touchWorldPos;
				Destroy(newTarget, 1.0f);
				move(touchWorldPos);
				// Only play sounds for localPlayer
				G.get().gameController.PlayAudio(sfx_player_target);
    		}
		} else if (Input.GetMouseButton(0)) {
			if (isAiming) {
				aimVec = tapBegin - Input.mousePosition;
				Direction newFacing = Direction.get(aimVec);
				lazyUpdateFacing(newFacing);
				arrowIcon.transform.localRotation = getAimRotation();
    		}
		} else if (Input.GetMouseButtonUp(0)) {
			if (isAiming) {
				// Called from the client but invoked on the server.
	            CmdAttack(transform.position, aimVec);
				isAiming = false;
				arrowIcon.SetActive(isAiming);
    		}
        }

        // Move towards destination
		Vector2 currPos = transform.position;
		Vector2 toDestination = destination - currPos;
		float distance = toDestination.sqrMagnitude;
		if (distance > 0.001f) {
			Vector2 moveVec = toDestination;
			if (distance > G.get().PLAYER_MOVE_SPEED * G.get().PLAYER_MOVE_SPEED) {
				toDestination.Normalize();
				moveVec = toDestination * G.get().PLAYER_MOVE_SPEED;
				walk(moveVec);
			} else {
				stop(destination);
			}
		}
	}

	public override void OnStartServer() {
		// animator needs to exist before networkFacing can take effect.
		animator = GetComponent<Animator>();
		networkFacing = facing.toInt();
		networkAnimAction = animAction;
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

    [Command]
	void CmdAttack(Vector3 position, Vector2 clientAimVec)
    {
		this.aimVec = clientAimVec;
		// Debug.Log("Attack");
		// create the arrow object locally
        var arrow = (GameObject)Instantiate(
            arrowPrefab,
			position,
			getAimRotation());
		// arrow.transform.parent = world.transform;

		// make the arrow move away in front of the player
		Vector2 arrowDirection = clientAimVec;
		arrowDirection.Normalize();
		arrow.GetComponent<Rigidbody2D>().velocity = arrowDirection * G.get().ARROW_SPEED;

		// spawn the arrow on the clients
		NetworkServer.Spawn(arrow);
        
		// make arrow disappear after 2 seconds
		Destroy(arrow, G.get().ARROW_LIFE);
    }

    private Quaternion getAimRotation() {
		return Quaternion.AngleAxis(
				Mathf.Rad2Deg * Mathf.Atan2(aimVec.y, aimVec.x),
				Vector3.forward);
    }

    private void walk(Vector2 moveVec) {
		Vector3 moveVec3 = moveVec;
		rigidBody.MovePosition(transform.position + moveVec3);
		Direction newFacing = Direction.get(moveVec);
		lazyUpdateFacing(newFacing);
    }

	// Called by localPlayer and by other clients via HookFacingChanged.
    private void localUpdateFacing(Direction newFacing) {
		facing = newFacing;
		animator.Play(animAction + facing.ToString());
    }

    // Called by localPlayer to update facing lazily and tell server.
	// Also called by server to update network SyncVar which will then inform other clients.
    private void lazyUpdateFacing(Direction newFacing) {
		if (newFacing.toInt() != Direction.NONE.toInt()
    		&& networkFacing != newFacing.toInt()) {
			if (isServer) {
				// Changing SyncVar will call HookFacingChanged.
				networkFacing = newFacing.toInt();
			} else {
				CmdUpdateFacing(newFacing.toInt());
			}
			// local player updates more quickly
			localUpdateFacing(newFacing);
    	}
    }

	// Called by localPlayer and by other clients via HookAnimActionChanged.
	private void localUpdateAnimAction(string newAnimAction) {
		animAction = newAnimAction;
		animator.Play(animAction + facing.ToString());
    }

	// Called by localPlayer to update anim action lazily and tell server.
	// Also called by server to update network SyncVar which will then inform other clients.
    private void lazyUpdateAnimAction(string newAnimAction) {
		if (isServer && networkAnimAction != newAnimAction) {
			networkAnimAction = newAnimAction;
			localUpdateAnimAction(networkAnimAction);
		} else if (animAction != newAnimAction) {
			CmdUpdateAnimAction(newAnimAction);
			// local player updates more quickly
			localUpdateAnimAction(newAnimAction);
		}
    }

	private void stop(Vector2 position) {
		destination = position;
		rigidBody.MovePosition(destination);
		lazyUpdateFacing(facing);
		lazyUpdateAnimAction("player_idle_");
    }

	private void move(Vector2 newDestination) {
		destination = newDestination;
		Vector2 curr_pos = transform.position;
		Direction direction = Direction.get(destination - curr_pos);
		lazyUpdateFacing(direction);
		lazyUpdateAnimAction("player_walk_");
    }

    // Called on all clients when var changes on server.
	private void HookFacingChanged(int newFacing) {
		// Local player should have updated.
		if (isLocalPlayer) {
			return;
		}
		localUpdateFacing(Direction.fromInt(newFacing));
	}

	// Called on all clients when var changes on server.
	private void HookAnimActionChanged(string newAnimAction) {
		// Local player should have updated.
		if (isLocalPlayer) {
			return;
		}
		localUpdateAnimAction(newAnimAction);
	}

	// Run on server to update facing, which will then tell other clients to update facing.
	[Command]
	void CmdUpdateFacing(int newFacing) {
		networkFacing = newFacing;
	}

	// Run on server to update anim, which will then tell other clients to update anim.
	[Command]
	void CmdUpdateAnimAction(string newAnimAction) {
		networkAnimAction = newAnimAction;
	}

//	[Command]
//	void CmdStop(Vector2 position, int newFacing) {
//		// Debug.Log("CmdStop at position: " + position + " facing: " + newFacing);
//		destination = position;
//		rigidBody.MovePosition(destination);
//		lazyUpdateFacing(Direction.fromInt(newFacing));
//		lazyUpdateAnimAction("player_idle_");
//	}

//	[Command]
//	void CmdMove(Vector2 newDestination) {
//		// Debug.Log("Move to dest " + newDestination);
//		destination = newDestination;
//		Vector2 curr_pos = transform.position;
//		Direction direction = Direction.get(destination - curr_pos);
//		lazyUpdateFacing(direction);
//		lazyUpdateAnimAction("player_walk_");
//	}
}
