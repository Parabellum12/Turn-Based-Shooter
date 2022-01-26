using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoMagazineScriptableObject", menuName = "ScriptableObjects/Weapons/Magazines/AmmoMag")]
public class AmmoMag : ScriptableObject
{
    public enum AmmoType
    {
        rifle,
        sniper,
        shotgun,
        pistol
    }

    public AmmoType ammoType;
    public int maxAmmo;
    public int currentAmmo;
}
