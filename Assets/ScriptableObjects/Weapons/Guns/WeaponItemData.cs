using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/Weapons/Guns/WeaponItemData")]
public class WeaponItemData : ScriptableObject
{
    public Item_Inventory_Data inventory_Data;
    public WeaponStats weapon_Stats;
    public string weaponName;
}
