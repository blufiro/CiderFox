using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ThiefBehaviour : EnemyBehaviour {

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
	Vector3 altarPosition;
	AltarBehavior altarBehavior;
	Vector3 targetPosition;
	Vector3 idleAnchorPosition;
	CarryOverheadBehaviour carryOverheadBehaviour;
	Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		state = ThiefState.LOCATE_CIDER;
		// For now, altar never moves, so target location never changes.
		var target = GameObject.FindGameObjectWithTag("AltarTag");
		altarPosition = target.transform.position;
		altarBehavior = target.GetComponent<AltarBehavior>();
		carryOverheadBehaviour = gameObject.GetComponent<CarryOverheadBehaviour>();
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isServer)
			return;

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
		if (altarBehavior.HasCider()) {
			changeState(ThiefState.MOVE_TO_CIDER);
		} else {
			changeState(ThiefState.IDLE_WHEN_NO_CIDER);
		}
	}

	private void UpdateIdleWhenNoCider() {
		MoveToTarget(targetPosition, ThiefState.LOCATE_CIDER);
	}

	private void UpdateMoveToCider() {
		if (!altarBehavior.HasCider()) {
			changeState(ThiefState.LOCATE_CIDER);
			return;
		}
		MoveToTarget(altarPosition, ThiefState.GRAB_CIDER);
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
			if (distance > G.get().THIEF_MOVE_SPEED * G.get().THIEF_MOVE_SPEED) {
				toDestination.Normalize();
				moveVec = toDestination * G.get().THIEF_MOVE_SPEED;
			}
			this.transform.Translate(moveVec);
		} else {
			changeState(onReachState);
		}
	}

	private void UpdateGrabCider() {
		if (!altarBehavior.HasCider()) {
			changeState(ThiefState.LOCATE_CIDER);
			return;
		}

		carryOverheadBehaviour.TakeItem(ciderPrefab);
		altarBehavior.gameObject.SendMessage("OnStealItem");
		changeState(ThiefState.RUN_AWAY);
	}

	private void Die() {
		Destroy(gameObject);
	}
}
