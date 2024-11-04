using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    public void DropCoins(int numCoins)
    {
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

    [CustomEditor(typeof(CoinPusher))]
    public class CoinPusherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CoinPusher script = (CoinPusher)target;
            if (GUILayout.Button("Drop 10 Coins"))
            {
                script.DropCoins(10);
            }
            if (GUILayout.Button("Drop 20 Coins"))
            {
                script.DropCoins(20);
            }
        }
    }
}
