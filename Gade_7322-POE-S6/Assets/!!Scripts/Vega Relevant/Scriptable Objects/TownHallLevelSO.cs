/*
    The TownHallLevelSO class defines level-specific settings for a town hall in a game. It allows configuration of shooting intervals, burst abilities, and other parameters.

    - Fields:
      - interval: Time between shots.
      - canShootFromAllPoints: Flag to enable shooting from all points.
      - hasBurstAbility: Flag to enable burst firing.
      - burstCount: Number of shots in a burst.
      - burstCooldown: Cooldown time between bursts.
      - burstActivationTime: Time before burst firing begins.

    - Methods:
      - ConfigureLevel(int level): Configures the town hall's settings based on the given level:
        - Level 0: Basic settings.
        - Level 1: Reduced interval, no burst ability.
        - Level 2: Reduced interval, shoot from all points, no burst ability.
        - Level 3: Further reduced interval, shoot from all points, burst ability enabled with set burst parameters.
*/

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
