using UnityEngine;

public class G
{
	public float ARROW_SPEED = 1000; // pixels per second
	public float ARROW_LIFE = 2.0f; // seconds

	private static G instance = new G();
	public static G get() {
		return instance;
	}
}
