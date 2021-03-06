using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileBuildData", menuName = "ScriptableObjects/WorldTiles/TileBuildData")]
public class TileBuildData : ScriptableObject
{
    public string BuildingName;
    public Sprite buildingSprite;
    public bool[] BulletBlockerSubGrid = new bool[9];
    public Vector2Int altasPos;
    public enum BuildingType
    {
        Ground,
        Decoration,
        Wall,

        DefaultTile
    };
    public BuildingType buildingType;


   
}
