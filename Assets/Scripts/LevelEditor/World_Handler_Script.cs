using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class World_Handler_Script : MonoBehaviour
{
    GridClass<WorldBuildTile> buildLevels = null;
    [SerializeField] float cellSize = 15f;
    [SerializeField] TMP_Dropdown mapSize;
    [SerializeField] int baseWidth = 60;
    [SerializeField] int baseHeight = 60;
    [SerializeField] TileBuildData defaultTile;
    public void GenerateNewMap() 
    {
        buildLevels = null;
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
        genNewMap(width, height);


    }

    private void genNewMap(int width, int height)
    {
        buildLevels = new GridClass<WorldBuildTile>(gameObject.transform, width, height, cellSize, Vector3.zero, (int x, int y) => new WorldBuildTile(x, y, defaultTile));
        setVisualGrid();
    }

    public void setTile(TileBuildData data, Vector2 mousePos, int buildLevel)
    {
        buildLevels.GetXY(mousePos, out int x, out int y);
        if (!buildLevels.inBounds(x,y))
        {
            return;
        }
        buildLevels.getGridObject(mousePos).setBuildData(data);
        buildLevels.triggerGridObjectChanged(x,y);
    }

    public GridClass<WorldBuildTile> getBuildLayers()
    {
        return buildLevels;
    }

    //visual handler
    List<TileVisual_Script> tileVisual_Scripts = new List<TileVisual_Script>();
    public void clearVisuals()
    {
        foreach ( TileVisual_Script scr in tileVisual_Scripts)
        {
            scr.deleteMe();
        }
        tileVisual_Scripts.Clear();
    }

    public void setVisualGrid()
    {
        if (buildLevels == null)
        {
            return;
        }
        setVisualGrid(0, buildLevels.width-1, buildLevels.height-1);
        Debug.Log("Finished Settig Visual Layering");
    }

    private void setVisualGrid(int layer, int x, int y)
    {
        if (!buildLevels.inBounds(x, y) || buildLevels.getGridObject(x,y).getDisplayOrder() != -1)
        {
            return;
        }
        Debug.Log("Layer:" + layer + "; coords:" + x + "," + y);
        buildLevels.getGridObject(x,y).setDisplayOrder(layer);
        setVisualGrid(layer+1, x-1, y);
        setVisualGrid(layer+1, x, y-1);

    }






    public class WorldBuildTile
    {
        int x;
        int y;
        int displayOrder = -1;
        //floor
        TileBuildData GroundBuildData;

        //decoration
        TileBuildData DecorationBuildData;

        //walls
        //top
        TileBuildData WallTopBuildData;
        //bottom
        TileBuildData WallBottomBuildData;
        //left
        TileBuildData WallleftBuildData;
        //right
        TileBuildData WallRightBuildData;


        public WorldBuildTile(int x, int y, TileBuildData defaultTileLoc)
        {
            this.x = x;
            this.y = y;
            this.GroundBuildData = defaultTileLoc;
        }

        public void setDisplayOrder(int layer)
        {
            displayOrder = layer;
        }

        public int getDisplayOrder()
        {
            return displayOrder;
        }

        public void setBuildData(TileBuildData data)
        {
            GroundBuildData = data;
        }

        public override string ToString()
        {
            if (GroundBuildData == null)
            {
                return "null";
            }
            return GroundBuildData.BuildingName;
        }
    }

    
}
