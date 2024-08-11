using UnityEngine;

[CreateAssetMenu(fileName = "TownHallLevel", menuName = "ScriptableObjects/TownHallLevel", order = 1)]
public class TownHallLevelSO : ScriptableObject
{
    public float interval = 1f;
    public bool canShootFromAllPoints = false;
    public bool hasBurstAbility = false;
    public int burstCount = 3;
    public float burstCooldown = 30f;
    public float burstActivationTime = 10f;

    public void ConfigureLevel(int level)
    {
        switch (level)
        {
            case 0:
                interval = 1f;
                canShootFromAllPoints = false;
                hasBurstAbility = false;
                break;
            case 1:
                interval = 0.8f;
                canShootFromAllPoints = false;
                hasBurstAbility = false;
                break;
            case 2:
                interval = 0.8f;
                canShootFromAllPoints = true;
                hasBurstAbility = false;
                break;
            case 3:
                interval = 0.6f;
                canShootFromAllPoints = true;
                hasBurstAbility = true;
                burstCount = 3;
                burstCooldown = 30f;
                burstActivationTime = 10f;
                break;
        }
    }
}
