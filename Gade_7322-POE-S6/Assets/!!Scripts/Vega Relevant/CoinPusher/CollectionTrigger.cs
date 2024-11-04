using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        string rewardTag = other.gameObject.tag;
        switch (rewardTag)
        {
            case "CoinPusherGold":
                GoldManager.instance.AddGold(20);
                Destroy(other.gameObject);
                break;
            case "CoinPusherSword":
                Debug.Log("Sword Collected");
                Destroy(other.gameObject);
                break;
            case "CoinPusherShield":
                Debug.Log("Shield Collected");
                Destroy(other.gameObject);
                break;
            default:
                break;
        }
        
    }
}
