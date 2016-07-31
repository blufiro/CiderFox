using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ThiefBehaviour : EnemyBehaviour {

	GameObject target;

	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("AltarTag");
	}
	
	// Update is called once per frame
	void Update () {
		// Move to target.
		Vector2 toDestination = target.transform.position - this.transform.position;
		float distance = toDestination.sqrMagnitude;
		if (distance > 0.001f) {
			Vector2 moveVec = toDestination;
			if (distance > G.get().THIEF_MOVE_SPEED * G.get().THIEF_MOVE_SPEED) {
				toDestination.Normalize();
				moveVec = toDestination * G.get().THIEF_MOVE_SPEED;
			}
			this.transform.Translate(moveVec);
		}
	}


}
