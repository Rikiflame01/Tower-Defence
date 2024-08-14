using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    private List<GameObject> enemies = new List<GameObject>();

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
    }

    private void OnDisable()
    {
        EventManager.instance.onEnemySpawned.RemoveListener(AddEnemy);
    }

    private void AddEnemy(GameObject enemy)
    {
        if (enemy.activeInHierarchy)
        {
            enemies.Add(enemy);
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
