using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public struct Wave
{
	public enum SpawnLocation {
		SCATTERED,
		TOP_RIGHT,
		TOP_LEFT,
		BOTTOM_RIGHT,
		BOTTOM_LEFT,
	}

	public float minute;
	public int numEnemies;
	public float enemySpeedMultiplier;
	public SpawnLocation spawnLocation;
}

