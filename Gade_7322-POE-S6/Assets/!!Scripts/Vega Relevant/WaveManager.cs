using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GridGenerator gridGenerator;
    public PathGenerator pathGenerator;
    public float spawnRadius = 1.0f;
    private int roundCounter = 0;

    private void Start()
    {
        EventManager.instance.onWaveMode.AddListener(StartWave);
    }

    private void StartWave()
    {
        roundCounter++;
        int enemyCount = roundCounter * 8;
        StartCoroutine(SpawnEnemies(enemyCount));
    }

    private IEnumerator SpawnEnemies(int count)
    {
        yield return new WaitUntil(() => gridGenerator != null && gridGenerator.IsGridGenerated());

        List<Vector3> startPositions = pathGenerator.startPositions;

        if (startPositions.Count == 0)
        {
            Debug.LogError("No start positions found for enemy spawning.");
            yield break;
        }

        int spawnCount = 0;
        int pathsCount = startPositions.Count;
        int enemiesPerPath = count / pathsCount;
        int remainder = count % pathsCount;

        for (int i = 0; i < pathsCount; i++)
        {
            int enemiesToSpawn = enemiesPerPath + (i < remainder ? 1 : 0);
            for (int j = 0; j < enemiesToSpawn; j++)
            {
                Vector3 spawnPosition = GetValidSpawnPosition(startPositions[i]);
                if (spawnPosition != Vector3.zero)
                {
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                    spawnCount++;
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private Vector3 GetValidSpawnPosition(Vector3 startPosition)
    {
        Vector3 randomPosition = startPosition + Random.insideUnitSphere * spawnRadius;
        randomPosition.y = startPosition.y;

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }
}
