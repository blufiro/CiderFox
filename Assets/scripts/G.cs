using UnityEngine;
using System.Collections.Generic;

public class G
{
	public const int NUM_PLAYERS = 2;
	public const int SCREEN_WIDTH = 640;
	public const int SCREEN_HEIGHT = 360;
	public const int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
	public const int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;
	public const int WORLD_WIDTH = 1280;
	public const int WORLD_HEIGHT = 720;
	public const int HALF_WORLD_WIDTH = WORLD_WIDTH / 2;
	public const int HALF_WORLD_HEIGHT = WORLD_HEIGHT / 2;
	public const int MAX_AUDIO_SOURCES = 16;

	public const int SAFE_SPAWN_RADIUS = G.HALF_WORLD_WIDTH + G.HALF_WORLD_HEIGHT;
	public const float SPAWN_RAND_EXTRA_RADIUS = 300;
	public float ARROW_SPEED = 1000; // pixels per second
	public float ARROW_LIFE = 1.0f; // seconds
	public int ARROW_DAMAGE = 1;
	public float PLAYER_MOVE_SPEED = 100.0f/60.0f; // pixels per second
	public float THIEF_MOVE_SPEED = 90.0f/60.0f; // pixels per second
	public int MAX_SPAWN_ATTEMPTS = 5;
	public float LIGHT_OFF_DELAY = 5.0f; //seconds
	public float CIDER_PRODUCE_DELAY = 30.0f; // seconds
	public int CARRY_OVERHEAD_OFFSET_Y = 4; // pixels
	public float GOD_ANGRY_DURATION = 3 * 60; // seconds
	public float SNAP_TIME_THRESHOLD = 1; // seconds

	public GameController gameController;

	private static G instance = new G();
	public static G get() {
		return instance;
	}

	public static Vector2 RandCircle(float radius) {
		float randAngle = Random.Range(0.0f, 2 * Mathf.PI);
        return new Vector2(
			radius * Mathf.Cos(randAngle),
			radius * Mathf.Sin(randAngle));
	}

	public static Vector2 RandVec2(float axisLength) {
		return new Vector2(
			Random.value * axisLength,
			Random.value * axisLength);
	}
}
