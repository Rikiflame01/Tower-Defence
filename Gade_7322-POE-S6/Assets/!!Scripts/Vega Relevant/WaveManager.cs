using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Range(0, 1)] private float standardWarriorRatio = 0.5f;
    [Range(0, 1)] private float wizardRatio = 0.3f;
    [Range(0, 1)] private float tankyKnightRatio = 0.2f;

    [SerializeField] private bool testDynamicRatioSystem = false; //Toggle to test dynamic ratio system

    private void Start()
    {
        EventManager.instance.onWaveMode.AddListener(StartWave);
    }

    private void StartWave()
    {
        roundCounter++;

        int enemyCount = roundCounter + 8;

        if (roundCounter > 2)
        {
            enemyCount = (roundCounter - 2) * 8;
        }

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

        //Apply dynamic ratio system if round is 25 or above, or if the testing toggle is active
        if (roundCounter >= 25 || testDynamicRatioSystem)
        {
            AdjustEnemyRatiosBasedOnPlayerBuildings();
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

    private void AdjustEnemyRatiosBasedOnPlayerBuildings()
    {
        int burstDefenders = GameObject.FindGameObjectsWithTag("BurstDefender").Length;
        int catapultDefenders = GameObject.FindGameObjectsWithTag("CatapultDefender").Length;
        int shieldDefenders = GameObject.FindGameObjectsWithTag("ShieldDefender").Length;

        int totalDefenders = burstDefenders + catapultDefenders + shieldDefenders;

        if (totalDefenders == 0)
        {
            Debug.LogWarning("No defenders found in the scene.");
            return;
        }

        // Ratio of each type of tower
        float burstRatio = (float)burstDefenders / totalDefenders;
        float catapultRatio = (float)catapultDefenders / totalDefenders;
        float shieldRatio = (float)shieldDefenders / totalDefenders;

        standardWarriorRatio = catapultRatio; // More catapults -> more standard warriors
        wizardRatio = shieldRatio;            // More shield defenders -> more wizards
        tankyKnightRatio = burstRatio;        // More burst towers -> more tanky knights

        Debug.Log($"Adjusted Enemy Ratios - Standard Warriors: {standardWarriorRatio}, Wizards: {wizardRatio}, Tanky Knights: {tankyKnightRatio}");

        float totalRatio = standardWarriorRatio + wizardRatio + tankyKnightRatio;
        standardWarriorRatio /= totalRatio;
        wizardRatio /= totalRatio;
        tankyKnightRatio /= totalRatio;

        Debug.Log($"Normalized Ratios - Standard Warriors: {standardWarriorRatio}, Wizards: {wizardRatio}, Tanky Knights: {tankyKnightRatio}");
    }
}
