using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public float maxDamage;
    public float maxDamageRange;
    public float maxRange;
    public enum weaponType
    {
        Rifle,
        Sniper_Rifle,
        Shotgun,
        Pistol,
        LMG
    }
    public weaponType weapon_Type;

    public float getDamage(float dist)
    {
        if (dist <= maxDamageRange)
        {
            return maxDamage;
        }
        else
        {
            return maxDamage - getDamageDropoff(maxDamageRange-dist);
        }
    }


    private float getDamageDropoff(float remainingDist)
    {
        float dropoffPer = maxDamage / (maxRange - maxDamageRange);
        return dropoffPer * remainingDist;
    }
}
