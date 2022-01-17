using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Inventory_Data", menuName = "ScriptableObjects/Item_Inventory_Data")]
public class Item_Inventory_Data : ScriptableObject
{
    public int width;
    public int height;
    public Sprite inventorySprite;
}
