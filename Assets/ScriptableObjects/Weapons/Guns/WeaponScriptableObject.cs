using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapons/Guns/WeaponScriptableObject")]
public class WeaponScriptableObject : ScriptableObject
{
    public string WeaponName = "";
    [TextArea(5, 5)] public string WeaponDescription = "";
    public float maxDamage;
    public float maxDamageRange;
    public float maxRange;
    //add ammo class object to act as mag for weapon
    public AmmoMag magazine;
    public AmmoMag.AmmoType validAmmo;



    public float getDamage(float distance)
    {
        if (distance <= maxDamageRange)
        {
            return maxDamage;
        }
        else
        {
            return maxDamage - getDamageDropoff(distance - maxDamageRange);
        }
    }

    private float getDamageDropoff(float dist)
    {
        float totalDropoffDist = maxRange - maxDamageRange;
        float damageDropOffWithDist = maxDamage / totalDropoffDist;
        return damageDropOffWithDist * dist;
    }
}
