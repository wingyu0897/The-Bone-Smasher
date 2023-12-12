using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionManager : MonoBehaviour
{
    public static SpawnPositionManager Instance;

	public List<Transform> spawnPositions;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	public Vector3 GetPosition()
	{
		return spawnPositions[Random.Range(0, spawnPositions.Count)].position;
	}
}
