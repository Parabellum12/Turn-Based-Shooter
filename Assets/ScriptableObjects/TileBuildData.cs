using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileBuildData", menuName = "ScriptableObjects/WorldTiles/TileBuildData")]
public class TileBuildData : ScriptableObject
{
    public string BuildingName;
    public Sprite buildingSprite;
    public bool[] BulletSubGrid = new bool[9];
}
