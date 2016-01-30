using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Direction
{
	public static Direction NONE = new Direction(0, 0.0f, 0.0f);
	public static Direction UP = new Direction(1, 0.0f, -1.0f);
	public static Direction UP_LEFT = new Direction(2, -1.0f, -1.0f);
	public static Direction UP_RIGHT = new Direction(3, 1.0f, -1.0f);
	public static Direction LEFT = new Direction(4, -1.0f, 0.0f);
	public static Direction RIGHT = new Direction(5, 1.0f, 0.0f);
	public static Direction DOWN = new Direction(6, 0.0f, 1.0f);
	public static Direction DOWN_LEFT = new Direction(7, -1.0f, 1.0f);
	public static Direction DOWN_RIGHT = new Direction(8, 1.0f, 1.0f);

	private static Direction[] directionsArray = new Direction[9] {
		NONE,
		UP,
		UP_LEFT,
		UP_RIGHT,
		LEFT,
		RIGHT,
		DOWN,
		DOWN_LEFT,
		DOWN_RIGHT,
	};

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

	public static Direction fromInt(int index) {
		return directionsArray[index];
	}

	[SerializeField]
	private int m_index;
	private Vector2 m_xy;
	private Vector3 m_xyz;
	private Direction(int index, float x, float y) {
		m_index = index;
		m_xy = new Vector2(x,y);
		m_xyz = new Vector3(x,y,0);
	}

	public int toInt() {
		return m_index;
	}

	public Vector2 toVector2() {
		return m_xy;
	}

	public Vector3 toVector3() {
		return m_xyz;
	}
}
