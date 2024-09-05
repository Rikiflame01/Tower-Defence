/*
    The TownHallProjectileSO class defines projectile properties and progression for a town hall in a game. It includes damage, special abilities, and methods to get the damage and reset properties.

    - Fields:
      - level: The current upgrade level of the projectile.
      - baseDamage: The base damage of the projectile.
      - hasHoming: Indicates if the projectile has homing capabilities.
      - hasRicochet: Indicates if the projectile can ricochet.
      - ricochetCount: Number of times the projectile can ricochet.
      - hasExplosion: Indicates if the projectile has an explosion effect.

    - Methods:
      - GetDamage(): Calculates and returns the projectile's damage based on its level and abilities.
        - Level 1: +3 damage.
        - Level 2: Adds homing and +3 damage.
        - Level 3: Adds ricochet with 1 ricochet count.
        - Level 4: +3 damage and increases ricochet count.
        - Level 5: Adds explosion effect.

      - Reset(): Resets all properties to their default values.
*/


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
    public void Reset()
    {
        level = 0;
        baseDamage = 5f;
        hasHoming = false;
        hasRicochet = false;
        ricochetCount = 0;
        hasExplosion = false;
    }
}
