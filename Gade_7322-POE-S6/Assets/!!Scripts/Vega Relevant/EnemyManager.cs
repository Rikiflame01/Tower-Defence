using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    private List<GameObject> enemies = new List<GameObject>();
    private bool spawningStopped = false;
    private float spawnCheckInterval = 5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.instance.onEnemySpawned.AddListener(AddEnemy);
        EventManager.instance.onWaveMode.AddListener(OnWaveModeStart);
    }

    private void OnDisable()
    {
        EventManager.instance.onEnemySpawned.RemoveListener(AddEnemy);
        EventManager.instance.onWaveMode.RemoveListener(OnWaveModeStart);
    }

    private void AddEnemy(GameObject enemy)
    {
        if (enemy.activeInHierarchy)
        {
            enemies.Add(enemy);
            spawningStopped = false;
            StopCoroutine(CheckForWaveEnd());
        }
    }

    private void OnWaveModeStart()
    {
        StartCoroutine(CheckForWaveEnd());
    }

    private IEnumerator CheckForWaveEnd()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnCheckInterval);

            if (enemies.Count == 0 && spawningStopped)
            {
                EventManager.instance.TriggerCooldownMode();
                yield break;
            }
            spawningStopped = true;
        }
    }

    public GameObject GetNearestEnemy(Vector3 position)
    {
        GameObject nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector3.Distance(position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy);
    }
}
