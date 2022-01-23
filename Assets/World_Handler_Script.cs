using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class World_Handler_Script : MonoBehaviour
{
    GridClass<BulletFireLine> bulletPath = null;
    List<GridClass<WorldBuildTile>> buildLevels = new List<GridClass<WorldBuildTile>>();

    [SerializeField] float cellSize = 10f;
    [SerializeField] TMP_Dropdown mapSize;
    [SerializeField] TMP_InputField mapHeight;
    [SerializeField]int baseWidth = 60;
    [SerializeField]int baseHeight = 60;
    public void GenerateNewMap() 
    {
        buildLevels.Clear();
        bulletPath = null;
        int width = 0;
        int height = 0;
        switch (mapSize.value)
        {
            case 0:
                width = baseWidth;
                height = baseHeight;
                break;
            case 1:
                width = 2 * baseWidth;
                height = 2 * baseHeight;
                break;
            case 2:
                width = 3 * baseWidth;
                height = 3 * baseHeight;
                break;
        };
        for (int i = 0; i < int.Parse(mapHeight.text); i++)
        {
            buildLevels.Add(new GridClass<WorldBuildTile>(gameObject.transform, width, height, cellSize, Vector3.zero, (int x, int y) => new WorldBuildTile(x, y)));
        }



    }





    public class WorldBuildTile
    {
        int x;
        int y;
        public WorldBuildTile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return x + "," + y;
        }
    }

    public class BulletFireLine
    {

        public BulletFireLine()
        {

        }
    }
}
