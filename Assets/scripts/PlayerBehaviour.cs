using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	public GameObject arrowPrefab;
	public GameObject arrowIcon;
	public GameObject target;

	public AudioClip sfx_player_target;

	private string anim_action = "player_idle_";
	private Direction facing = Direction.DOWN;
	[SyncVar(hook="FacingChanged")]
	private int networkFacing;

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
    			CmdStop(transform.position, facing.toInt());
    		} else {
				isAiming = false;
				arrowIcon.SetActive(isAiming);
				// Only instantiate for local player
				GameObject newTarget = Instantiate(target);
				// newTarget.transform.parent = world.transform;
				newTarget.transform.position = touchWorldPos;
				Destroy(newTarget, 1.0f);
				CmdMove(touchWorldPos);
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
	            CmdAttack();
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
				stopAtDestination();
			}


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

    [Command]
	void CmdAttack()
    {
		// Debug.Log("Attack");
		// create the arrow object locally
        var arrow = (GameObject)Instantiate(
            arrowPrefab,
			transform.position,
			getAimRotation());
		// arrow.transform.parent = world.transform;

		// make the arrow move away in front of the player
		arrow.GetComponent<Rigidbody2D>().velocity = aimVec * G.get().ARROW_SPEED;

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
    private void lazyUpdateFacing(Direction newFacing) {
		if (newFacing.toInt() != Direction.NONE.toInt()
    		&& networkFacing != newFacing.toInt()) {
    		CmdUpdateFacing(newFacing.toInt());
    	}
    }
	private void stopAtDestination() {
    	rigidBody.MovePosition(destination);
		CmdStop(destination, facing.toInt());
    }

    private void updateFacing (int newFacing) {
		// Debug.Log("update facing newFacing: " + newFacing + " from : " + networkFacing);
    	networkFacing = newFacing;
    	facing = Direction.fromInt(networkFacing);
		animator.Play(anim_action + facing.ToString());
    }

    [Client]
	void FacingChanged(int newFacing) {
		facing = Direction.fromInt(newFacing);
		animator.Play(anim_action + facing.ToString());
	}

	[Command]
	void CmdUpdateFacing(int newFacing) {
		updateFacing(newFacing);
    }

	[Command]
	void CmdStop(Vector2 position, int newFacing) {
		// Debug.Log("CmdStop at position: " + position + " facing: " + newFacing);
		destination = position;
		rigidBody.MovePosition(destination);
		updateFacing(newFacing);
		anim_action = "player_idle_";
		animator.Play(anim_action + facing.ToString());
	}

	[Command]
	void CmdMove(Vector2 newDestination) {
		// Debug.Log("Move to dest " + newDestination);
		destination = newDestination;
		Vector2 curr_pos = transform.position;
		Direction direction = Direction.get(destination - curr_pos);
		updateFacing(direction.toInt());
		// Debug.Log("play with direction: player_walk_" + direction.name());
		anim_action = "player_walk_";
		animator.Play(anim_action + facing.ToString());
	}
}
