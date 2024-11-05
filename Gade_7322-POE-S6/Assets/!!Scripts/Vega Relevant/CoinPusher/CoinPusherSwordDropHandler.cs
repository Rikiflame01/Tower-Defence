using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPusherSwordDropHandler : MonoBehaviour
{
    private int roundCounter = 0;
    private int enemiesToEliminate = 0;
    private int totalWaveEnemies = 0;
    private bool isProcessing = false;

    public GameObject vfxPrefab;
    public int rewardMinAmount = 1;
    public int rewardMaxAmount = 5;

    private void OnEnable()
    {
        EventManager.instance.onCoinPusherSwordDrop.AddListener(OnCoinPusherSwordDrop);
        EventManager.instance.onWaveMode.AddListener(OnWaveMode);
    }

    private void OnDisable()
    {
        EventManager.instance.onCoinPusherSwordDrop.RemoveListener(OnCoinPusherSwordDrop);
        EventManager.instance.onWaveMode.RemoveListener(OnWaveMode);
    }

    private void OnWaveMode()
    {
        roundCounter++;

        if (roundCounter > 2)
        {
            totalWaveEnemies = (roundCounter - 2) * 8;
        }
        else
        {
            totalWaveEnemies = roundCounter + 8;
        }
    }

    private void OnCoinPusherSwordDrop()
    {
        int newEnemiesToEliminate = Mathf.CeilToInt(totalWaveEnemies / 4f);

        enemiesToEliminate += newEnemiesToEliminate;

        if (!isProcessing)
        {
            StartCoroutine(EliminateEnemiesInIntervals());
        }
    }

    private IEnumerator EliminateEnemiesInIntervals()
    {
        isProcessing = true;

        while (enemiesToEliminate > 0)
        {
            if (GameManager.instance.GetCurrentState() == GameManager.GameState.Wave)
            {
                List<GameObject> enemies = EnemyManager.instance.GetAllEnemies();

                if (enemies.Count > 0)
                {
                    int enemiesToRemoveNow = Mathf.Min(2, enemiesToEliminate, enemies.Count);

                    for (int i = 0; i < enemiesToRemoveNow; i++)
                    {
                        enemies = EnemyManager.instance.GetAllEnemies();

                        if (enemies.Count > 0)
                        {
                            GameObject enemy = enemies[0];

                            if (vfxPrefab != null)
                            {
                                Renderer enemyRenderer = enemy.GetComponent<Renderer>();
                                Vector3 vfxPosition = enemy.transform.position;

                                if (enemyRenderer != null)
                                {
                                    vfxPosition.y = enemyRenderer.bounds.center.y;
                                }

                                Instantiate(vfxPrefab, vfxPosition, Quaternion.identity);
                            }
                            int rewardAmount = Random.Range(rewardMinAmount, rewardMaxAmount + 1);
                            GoldDropper goldDropper = enemy.GetComponent<GoldDropper>();
                            if (goldDropper != null)
                            {
                                goldDropper.DropGold(rewardAmount);
                            }

                            EnemyManager.instance.RemoveEnemy(enemy);
                            Debug.Log("Enemy removed: " + enemy.tag);
                            enemiesToEliminate--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    yield return StartCoroutine(WaitForNextWave());
                }
            }
            else
            {
                yield return new WaitForSeconds(5f);
            }
        }

        isProcessing = false;
    }

    private IEnumerator WaitForNextWave()
    {
        while (GameManager.instance.GetCurrentState() != GameManager.GameState.Wave)
        {
            yield return new WaitForSeconds(5f);
        }
    }
}
