using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    public GameObject standardWarriorPrefab;
    public GameObject wizardPrefab;
    public GameObject tankyKnightPrefab;
    public GridGenerator gridGenerator;
    public PathGenerator pathGenerator;
    public float spawnRadius = 1.0f;
    public int roundCounter = 0;
    public float baseSpawnDelay = 1f;

    [Range(0, 1)] public float standardWarriorRatio = 0.5f;
    [Range(0, 1)] public float wizardRatio = 0.3f;
    [Range(0, 1)] public float tankyKnightRatio = 0.2f;

    private void Start()
    {
        EventManager.instance.onWaveMode.AddListener(StartWave);
    }

    private void StartWave()
    {
        roundCounter++;

        int enemyCount = 8;

        if (roundCounter > 3)
        {
            enemyCount = roundCounter * 8;
        }

        float spawnDelay = Mathf.Max(0.1f, baseSpawnDelay - (roundCounter * 0.05f));

        int spawnPointsToUse = 1;
        if (roundCounter >= 25)
        {
            spawnPointsToUse = pathGenerator.startPositions.Count;
        }
        else if (roundCounter >= 15)
        {
            spawnPointsToUse = Mathf.Min(3, pathGenerator.startPositions.Count);
        }
        else if (roundCounter >= 10)
        {
            spawnPointsToUse = Mathf.Min(2, pathGenerator.startPositions.Count);
        }

        StartCoroutine(SpawnCustomWave(enemyCount, spawnDelay, spawnPointsToUse));
    }

    private IEnumerator SpawnCustomWave(int count, float delay, int spawnPointsToUse)
    {
        yield return new WaitUntil(() => gridGenerator != null && gridGenerator.IsGridGenerated());

        List<Vector3> startPositions = pathGenerator.startPositions;

        if (startPositions.Count == 0)
        {
            Debug.LogError("No start positions found for enemy spawning.");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < spawnPointsToUse; j++)
            {
                Vector3 spawnPosition = GetValidSpawnPosition(startPositions[j]);
                if (spawnPosition != Vector3.zero)
                {
                    GameObject enemyPrefab = SelectEnemyPrefabByRound();
                    GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                    RarityHandler rarityHandler = enemyInstance.GetComponent<RarityHandler>();
                    if (rarityHandler != null)
                    {
                        rarityHandler.SetAllowedRarities(roundCounter);
                    }

                    EventManager.instance.TriggerEnemySpawned(enemyInstance);
                }
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private GameObject SelectEnemyPrefabByRound()
    {
        if (roundCounter == 1)
        {
            return standardWarriorPrefab;
        }
        else if (roundCounter == 2)
        {
            return tankyKnightPrefab;
        }
        else if (roundCounter == 3)
        {
            return wizardPrefab;
        }
        else
        {
            return SelectEnemyPrefab();
        }
    }

    private GameObject SelectEnemyPrefab()
    {
        float randomValue = Random.value;

        if (randomValue < standardWarriorRatio)
        {
            return standardWarriorPrefab;
        }
        else if (randomValue < standardWarriorRatio + wizardRatio)
        {
            return wizardPrefab;
        }
        else
        {
            return tankyKnightPrefab;
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
