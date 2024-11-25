using UnityEngine;

public class CoinCollision : MonoBehaviour
{
    private static float globalCooldownTime = 0.2f;
    private static float lastGlobalCollisionTime;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("CoinPusherGold"))
        {
            if (Time.time - lastGlobalCollisionTime >= globalCooldownTime)
            {
                float random = Random.Range(3f, 4f);
                string[] coinDropSounds = { "CoinDrop1", "CoinDrop2", "CoinDrop3", "CoinDrop4", "CoinDrop5", "CoinDrop6", "CoinDrop7", "CoinDrop8", "CoinDrop9", "CoinDrop10" };
                string sound = coinDropSounds[Random.Range(0, coinDropSounds.Length)];

                SoundManager.Instance.PlaySFX(sound, random, transform.position);
                lastGlobalCollisionTime = Time.time;
            }
        }
    }
}
