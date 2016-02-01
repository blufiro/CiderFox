using UnityEngine;

public class G
{
	public const int SCREEN_WIDTH = 640;
	public const int SCREEN_HEIGHT = 360;
	public const int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
	public const int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;
	public const int WORLD_WIDTH = 1280;
	public const int WORLD_HEIGHT = 720;
	public const int HALF_WORLD_WIDTH = WORLD_WIDTH / 2;
	public const int HALF_WORLD_HEIGHT = WORLD_HEIGHT / 2;

	public float ARROW_SPEED = 1000; // pixels per second
	public float ARROW_LIFE = 1.0f; // seconds
	public int ARROW_DAMAGE = 1;
	public float PLAYER_MOVE_SPEED = 100.0f/60.0f; // pixels per second
	public int MAX_SPAWN_ATTEMPTS = 5;
	public float LIGHT_OFF_DELAY = 5.0f; //seconds
	public float CIDER_PRODUCE_DELAY = 30.0f; // seconds
	public int CARRY_OVERHEAD_OFFSET_Y = 4; // pixels
	public float GOD_ANGRY_DURATION = 10; // seconds

	private static G instance = new G();
	public static G get() {
		return instance;
	}
}
