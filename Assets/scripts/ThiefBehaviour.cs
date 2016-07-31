using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ThiefBehaviour : EnemyBehaviour {

	public ItemBehaviour ciderPrefab;

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
	Vector3 runAwayPosition;
	CarryOverheadBehaviour carryOverheadBehaviour;

	// Use this for initialization
	void Start () {
		state = ThiefState.LOCATE_CIDER;
		// For now, altar never moves, so target location never changes.
		var target = GameObject.FindGameObjectWithTag("AltarTag");
		altarPosition = target.transform.position;
		altarBehavior = target.GetComponent<AltarBehavior>();
		carryOverheadBehaviour = gameObject.GetComponent<CarryOverheadBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isServer)
			return;

		switch(state) {
			case ThiefState.LOCATE_CIDER:
				if (altarBehavior.HasCider()) {
					changeState(ThiefState.MOVE_TO_CIDER);
				} else {
					changeState(ThiefState.IDLE_WHEN_NO_CIDER);
				}
			break;
			case ThiefState.IDLE_WHEN_NO_CIDER: IdleWhenNoCider(); break;
			case ThiefState.MOVE_TO_CIDER: UpdateMoveToCider(); break;
			case ThiefState.GRAB_CIDER: GrabCider(); break;
			case ThiefState.RUN_AWAY: UpdateRunAway(); break;
			case ThiefState.RUN_AWAY_SUCCESS: UpdateRunAwaySuccess(); break;
			case ThiefState.DEAD: Die(); break;
		}
	}

	private void changeState(ThiefState newState) {
		state = newState;
		switch(state) {
			case ThiefState.LOCATE_CIDER: break;
			case ThiefState.IDLE_WHEN_NO_CIDER: break;
			case ThiefState.MOVE_TO_CIDER: break;
			case ThiefState.GRAB_CIDER: break;
			case ThiefState.RUN_AWAY: break;
			case ThiefState.DEAD: break;
		}
	}

	private void IdleWhenNoCider() {
		// TODO add idle movement

		// After some movement, go back into checking state.
		changeState(ThiefState.LOCATE_CIDER);
	}

	private void UpdateMoveToCider() {
		if (!altarBehavior.HasCider()) {
			changeState(ThiefState.LOCATE_CIDER);
			return;
		}
		MoveToTarget(altarPosition, ThiefState.GRAB_CIDER);
	}

	private void UpdateRunAway() {
		MoveToTarget(runAwayPosition, ThiefState.RUN_AWAY_SUCCESS);
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

	private void GrabCider() {
		if (!altarBehavior.HasCider()) {
			changeState(ThiefState.LOCATE_CIDER);
			return;
		}

		carryOverheadBehaviour.TakeItem(ciderPrefab);
		altarBehavior.gameObject.SendMessage("OnStealItem");
		
		// TODO Make this smarter by running away from players?
		float randAngle = Random.Range(0.0f, 2 * Mathf.PI);
        runAwayPosition = new Vector3(
			G.SAFE_SPAWN_RADIUS * Mathf.Cos(randAngle),
			G.SAFE_SPAWN_RADIUS * Mathf.Sin(randAngle),
			0.0f);
		changeState(ThiefState.RUN_AWAY);
	}

	private void Die() {
		Destroy(gameObject);
	}
}
