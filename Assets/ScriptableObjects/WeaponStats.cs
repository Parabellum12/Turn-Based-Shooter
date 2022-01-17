using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public float maxDamage;
    public float maxDamageRange;
    public float maxRange;
    public enum WeaponType
    {
        Rifle,
        Sniper_Rifle,
        Shotgun,
        Pistol,
        LMG
    }

    public enum ValidWeaponSlot
    {
        Primary,
        Secondary
    }

    public WeaponType weapon_Type;
    public ValidWeaponSlot valid_Weapon_Slot;
    [System.Serializable]
    public class firingModes
    {
        public enum mode
        {
            Semi,
            Burst,
            Auto,
        }
        public mode firingMode;
        public int maxShots;
        public float maxOffsetAngle;
        public float timeBetweenShots;
    }
    public firingModes[] firing_Modes;
    public int current_Firing_Mode = 0;




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


