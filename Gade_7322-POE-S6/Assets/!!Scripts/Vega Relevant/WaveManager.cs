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
    public int roundCounter = 0;
    public float baseSpawnDelay = 1f;

    private void Start()
    {
        EventManager.instance.onWaveMode.AddListener(StartWave);
    }

    private void StartWave()
    {
        roundCounter++;
        int enemyCount = roundCounter * 8;

        float spawnDelay = Mathf.Max(0.1f, baseSpawnDelay - (roundCounter * 0.05f));

        int spawnPointsToUse = 1;
        if (roundCounter >= 20)
        {
            spawnPointsToUse = pathGenerator.startPositions.Count;
        }
        else if (roundCounter >= 10)
        {
            spawnPointsToUse = Mathf.Min(3, pathGenerator.startPositions.Count);
        }
        else if (roundCounter >= 5)
        {
            spawnPointsToUse = Mathf.Min(2, pathGenerator.startPositions.Count);
        }

        StartCoroutine(SpawnEnemiesSimultaneously(enemyCount, spawnDelay, spawnPointsToUse));
    }

    private IEnumerator SpawnEnemiesSimultaneously(int count, float delay, int spawnPointsToUse)
    {
        yield return new WaitUntil(() => gridGenerator != null && gridGenerator.IsGridGenerated());

        List<Vector3> startPositions = pathGenerator.startPositions;

        if (startPositions.Count == 0)
        {
            Debug.LogError("No start positions found for enemy spawning.");
            yield break;
        }

        int enemiesPerPath = count / spawnPointsToUse;
        int remainder = count % spawnPointsToUse;

        for (int j = 0; j < enemiesPerPath + (remainder > 0 ? 1 : 0); j++)
        {
            for (int i = 0; i < spawnPointsToUse; i++)
            {
                Vector3 spawnPosition = GetValidSpawnPosition(startPositions[i]);
                if (spawnPosition != Vector3.zero)
                {
                    GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                    EventManager.instance.TriggerEnemySpawned(enemyInstance);
                }
            }
            yield return new WaitForSeconds(delay);
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
