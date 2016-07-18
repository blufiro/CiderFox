using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Direction
{
	public static Direction NONE = new Direction(0, 0.0f, 0.0f, "none");
	public static Direction RIGHT = new Direction(1, 1.0f, 0.0f, "east");
	public static Direction UP_RIGHT = new Direction(2, 1.0f, -1.0f, "northeast");
	public static Direction UP = new Direction(3, 0.0f, -1.0f, "north");
	public static Direction UP_LEFT = new Direction(4, -1.0f, -1.0f, "northwest");
	public static Direction LEFT = new Direction(5, -1.0f, 0.0f, "west");
	public static Direction DOWN_LEFT = new Direction(6, -1.0f, 1.0f, "southwest");
	public static Direction DOWN = new Direction(7, 0.0f, 1.0f, "south");
	public static Direction DOWN_RIGHT = new Direction(8, 1.0f, 1.0f, "southeast");

	private static Direction[] directionsArray = new Direction[9] {
		NONE,
		RIGHT,
		UP_RIGHT,
		UP,
		UP_LEFT,
		LEFT,
		DOWN_LEFT,
		DOWN,
		DOWN_RIGHT
	};

	private static float directionAngleRadians = Mathf.PI * 2.0f / 8.0f;

	public static Direction get(Vector2 vector) {
		float angleRadians = Mathf.Atan2(vector.y, vector.x);
		int angleIndex = (int) Mathf.Round(angleRadians / directionAngleRadians);
		// Wrap around
		if (angleIndex < 0)
			angleIndex += 8;
		// Debug.Log("Direction vector " + vector+" angle: " + angleRadians + " angleIndx: " + angleIndex);

		// + 1 because array starts with NONE
		return directionsArray[angleIndex + 1];
	}

	public static Direction fromInt(int index) {
		return directionsArray[index];
	}

	[SerializeField]
	private int m_index;
	private Vector2 m_xy;
	private Vector3 m_xyz;
	private string m_name;
	private Direction(int index, float x, float y, string name) {
		m_index = index;
		m_xy = new Vector2(x,y);
		m_xyz = new Vector3(x,y,0);
		m_name = name;
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

	public string name() {
		return m_name;
	}
}
