using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ThiefBehaviour : EnemyBehaviour {

	public GameObject deathPrefab;
	public ItemBehaviour ciderPrefab;
	public float idleRandomRadius;

	public enum ThiefState {
		LOCATE_CIDER,
		IDLE_WHEN_NO_CIDER,
		MOVE_TO_CIDER,
		GRAB_CIDER,
		RUN_AWAY,
		RUN_AWAY_SUCCESS,
		DEAD
	}
	// Public For debugging
	public ThiefState state;
	ItemSource ciderSource;
	Vector3 targetPosition;
	Vector3 idleAnchorPosition;
	CarryOverheadBehaviour carryOverheadBehaviour;

	// Use this for initialization
	void Start () {
		state = ThiefState.LOCATE_CIDER;
		ciderSource = null;
		carryOverheadBehaviour = gameObject.GetComponent<CarryOverheadBehaviour>();
	}

	void OnDefeat() {
		if (!isServer)
			return;

		if (carryOverheadBehaviour.IsCarryingItem()) {
			carryOverheadBehaviour.DropCarriedItem();
		}

		var deathGob = (GameObject)Instantiate(deathPrefab);
		deathGob.transform.position = transform.position;
		NetworkServer.Spawn(deathGob);
		Destroy(deathGob, 5.0f);
	}
	
	// Update is called once per frame
	public override void Update() {
		if (!isServer)
			return;
		base.Update();
		switch(state) {
			case ThiefState.LOCATE_CIDER: UpdateLocateCider(); break;
			case ThiefState.IDLE_WHEN_NO_CIDER: UpdateIdleWhenNoCider(); break;
			case ThiefState.MOVE_TO_CIDER: UpdateMoveToCider(); break;
			case ThiefState.GRAB_CIDER: UpdateGrabCider(); break;
			case ThiefState.RUN_AWAY: UpdateRunAway(); break;
			case ThiefState.RUN_AWAY_SUCCESS: UpdateRunAwaySuccess(); break;
			case ThiefState.DEAD: Die(); break;
		}
	}

	private void changeState(ThiefState newState) {
		if (state == newState) {
			return;
		}
		state = newState;
		switch(state) {
			case ThiefState.LOCATE_CIDER: break;
			case ThiefState.IDLE_WHEN_NO_CIDER: {
				// Only change anchor if too far away from previous target.
				if ((idleAnchorPosition - transform.position).sqrMagnitude > idleRandomRadius * idleRandomRadius) {
					idleAnchorPosition = transform.position; 
				}
				targetPosition = idleAnchorPosition + (Vector3) G.RandCircle(idleRandomRadius);
			}
			break;
			case ThiefState.MOVE_TO_CIDER: break;
			case ThiefState.GRAB_CIDER: break;
			case ThiefState.RUN_AWAY: {
				// TODO Make this smarter by running away from players?
				targetPosition = G.RandCircle(G.SAFE_SPAWN_RADIUS);
			}
			break;
			case ThiefState.DEAD: break;
		}
	}

	private void UpdateLocateCider() {
		if (G.get().ciderManager.HasCider()) {
			ciderSource = G.get().ciderManager.GetNearestCiderPos(transform.position);
			changeState(ThiefState.MOVE_TO_CIDER);
		} else {
			changeState(ThiefState.IDLE_WHEN_NO_CIDER);
		}
	}

	private void UpdateIdleWhenNoCider() {
		MoveToTarget(targetPosition, ThiefState.LOCATE_CIDER);
	}

	private void UpdateMoveToCider() {
		if (ciderSource == null || !ciderSource.HasItem()) {
			changeState(ThiefState.LOCATE_CIDER);
			return;
		}
		MoveToTarget(ciderSource.GetItemPosition(), ThiefState.GRAB_CIDER);
	}

	private void UpdateRunAway() {
		MoveToTarget(targetPosition, ThiefState.RUN_AWAY_SUCCESS);
	}

	private void UpdateRunAwaySuccess() {
		G.get().gameController.OnThiefRanAway();
		changeState(ThiefState.DEAD);
	}

	private void MoveToTarget(Vector3 targetPosition, ThiefState onReachState) {
		Vector2 toDestination = targetPosition - this.transform.position;
		float distance = toDestination.sqrMagnitude;
		if (distance > 0.001f) {
			Vector2 moveVec = toDestination;
			float speed = G.get().THIEF_MOVE_SPEED * speedMultiplier;
			if (distance > speed * speed) {
				toDestination.Normalize();
				moveVec = toDestination * speed;
			}
			this.transform.Translate(moveVec);
		} else {
			changeState(onReachState);
		}
	}

	private void UpdateGrabCider() {
		if (ciderSource == null || !ciderSource.HasItem()) {
			changeState(ThiefState.LOCATE_CIDER);
			return;
		}

		carryOverheadBehaviour.TakeItem(ciderPrefab);
		ciderSource.OnStealItem();
		changeState(ThiefState.RUN_AWAY);
	}

	private void Die() {
		Destroy(gameObject);
	}
}
