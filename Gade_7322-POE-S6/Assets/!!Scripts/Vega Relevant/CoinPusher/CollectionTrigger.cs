using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionTrigger : MonoBehaviour
{

    private string[] tags = { "ShieldDefender", "TownHall", "BurstDefender","CatapultDefender" };

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
                SoundManager.Instance.PlaySFX("key1", 2f, transform.position);
                RemoveEnemies();
                Destroy(other.gameObject);
                break;
            case "CoinPusherShield":
                SoundManager.Instance.PlaySFX("key1", 2f, transform.position);
                HealDefenders();
                Destroy(other.gameObject);
                break;
            default:
                break;
        }
        
    }

    void HealDefenders()
    {
        foreach (string tag in tags)
        {
            var objectsToHeal = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in objectsToHeal)
            {
                var health = obj.GetComponent<IHealth>();
                if (health != null)
                {
                    health.Heal();
                }
            } 
        }
    }

    void RemoveEnemies()
    {
        EventManager.instance.onCoinPusherSwordDrop.Invoke();
    }

}
