using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class CoinPusher : MonoBehaviour
{
    [Header("Pusher Settings")]
    public Transform pusher;
    public Transform targetPosition;
    private Vector3 originalPosition;
    public float pusherSpeed = 2f;
    private Rigidbody pusherRigidbody;

    [Header("Reward Spawn Settings")]
    public Transform[] spawnPointsArray1;
    public Transform[] spawnPointsArray2;

    [System.Serializable]
    public class Reward
    {
        public GameObject rewardObject;
        [Range(0f, 1f)]
        public float spawnProbability;
    }

    public Reward[] rewardsArray1;
    public Reward[] rewardsArray2;

    [Header("Coin Drop Settings")]
    public Transform[] coinDropTransforms;
    public GameObject coinPrefab;

    [Header("Wave Settings")]
    public int waveCounter = 0;

    private void OnEnable()
    {
        EventManager.instance.onDropCoin.AddListener(DropCoins);
        EventManager.instance.onWaveMode.AddListener(IncrementWave);
    }

    private void OnDisable()
    {
        EventManager.instance.onDropCoin.RemoveListener(DropCoins);
        EventManager.instance.onWaveMode.RemoveListener(IncrementWave);
    }

    private void Start()
    {
        originalPosition = pusher.position;
        pusherRigidbody = pusher.GetComponent<Rigidbody>();

        if (pusherRigidbody == null)
        {
            Debug.LogError("Pusher Rigidbody not found! Please add a Rigidbody component to the pusher object.");
            return;
        }
        else
        {
            pusherRigidbody.isKinematic = true;
            pusherRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            pusherRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        StartCoroutine(MovePusher());

        SpawnRewards();
    }

    private IEnumerator MovePusher()
    {
        while (true)
        {
            Vector3 target = Vector3.Distance(pusher.position, originalPosition) < 0.01f ? targetPosition.position : originalPosition;
            while (Vector3.Distance(pusher.position, target) > 0.01f)
            {
                Vector3 newPosition = Vector3.MoveTowards(pusher.position, target, pusherSpeed * Time.deltaTime);
                pusherRigidbody.MovePosition(newPosition);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void IncrementWave()
    {
        waveCounter++;
    }

    public void DropCoins()
    {
        if (waveCounter <= 0)
        {
            Debug.LogWarning("No waves left! Cannot drop coins.");
            return;
        }

        waveCounter--;

        int numCoins = Random.Range(10, 100);
        for (int i = 0; i < numCoins; i++)
        {
            int dropIndex = Random.Range(0, coinDropTransforms.Length);
            Instantiate(coinPrefab, coinDropTransforms[dropIndex].position, Quaternion.identity);
        }

    }

    private void SpawnRewards()
    {
        for (int i = 0; i < spawnPointsArray1.Length; i++)
        {
            Reward rewardToSpawn = GetRandomReward(rewardsArray1);
            if (rewardToSpawn != null)
            {
                Instantiate(rewardToSpawn.rewardObject, spawnPointsArray1[i].position, Quaternion.identity);
            }
        }

        for (int i = 0; i < spawnPointsArray2.Length; i++)
        {
            Reward rewardToSpawn = GetRandomReward(rewardsArray2);
            if (rewardToSpawn != null)
            {
                Instantiate(rewardToSpawn.rewardObject, spawnPointsArray2[i].position, Quaternion.identity);
            }
        }
    }

    private Reward GetRandomReward(Reward[] rewardsArray)
    {
        float totalProbability = 0f;
        foreach (Reward reward in rewardsArray)
        {
            totalProbability += reward.spawnProbability;
        }

        float randomPoint = Random.value * totalProbability;

        foreach (Reward reward in rewardsArray)
        {
            if (randomPoint < reward.spawnProbability)
            {
                return reward;
            }
            else
            {
                randomPoint -= reward.spawnProbability;
            }
        }
        return null;
    }

}
