using UnityEngine;

public sealed class Direction
{
	public static Direction NONE = new Direction(0.0f, 0.0f);
	public static Direction UP = new Direction(0.0f, -1.0f);
	public static Direction UP_LEFT = new Direction(-1.0f, -1.0f);
	public static Direction UP_RIGHT = new Direction(1.0f, -1.0f);
	public static Direction LEFT = new Direction(-1.0f, 0.0f);
	public static Direction RIGHT = new Direction(1.0f, 0.0f);
	public static Direction DOWN = new Direction(0.0f, 1.0f);
	public static Direction DOWN_LEFT = new Direction(-1.0f, 1.0f);
	public static Direction DOWN_RIGHT = new Direction(1.0f, 1.0f);

	public static Direction get(Vector2 vector) {
		return get(vector, 0.0f);
	}

	public static Direction get(Vector2 vector, float minThreshold) {
		if (vector.x < minThreshold)
			if (vector.y < minThreshold) return UP_LEFT;
			else if (vector.y > minThreshold) return DOWN_LEFT;
			else return LEFT;
		else if (vector.x > minThreshold)
			if (vector.y < minThreshold) return UP_RIGHT;
			else if (vector.y > minThreshold) return DOWN_RIGHT;
			else return RIGHT;
		else if (vector.y < minThreshold) return UP;
		else if (vector.y > minThreshold) return DOWN;
		else return NONE;
	}

	private Vector2 xy;
	private Vector3 xyz;
	private Direction(float x, float y) {
		xy = new Vector2(x,y);
		xyz = new Vector3(x,y,0);
	}

	public Vector2 toVector2() {
		return xy;
	}

	public Vector3 toVector3() {
		return xyz;
	}
}
