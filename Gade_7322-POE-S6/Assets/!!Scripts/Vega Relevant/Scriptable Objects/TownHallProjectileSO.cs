using UnityEngine;

[CreateAssetMenu(fileName = "TownHallProjectile", menuName = "ScriptableObjects/TownHallProjectile", order = 1)]
public class TownHallProjectileSO : ScriptableObject
{
    public int level;
    public float baseDamage = 5f;
    public bool hasHoming = false;
    public bool hasRicochet = false;
    public int ricochetCount = 0;
    public bool hasExplosion = false;

    public float GetDamage()
    {
        float damage = baseDamage;

        if (level >= 1)
        {
            damage += 3f;
        }

        if (level >= 2)
        {
            hasHoming = true;
            damage += 3f;
        }

        if (level >= 3)
        {
            hasRicochet = true;
            ricochetCount = 1;
        }

        if (level >= 4)
        {
            damage += 3f;
            ricochetCount++;
        }

        if (level >= 5)
        {
            hasExplosion = true;
        }

        return damage;
    }
}
