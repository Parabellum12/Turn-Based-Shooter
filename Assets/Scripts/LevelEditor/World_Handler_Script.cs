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
    public static TileBuildData[] allTileBuildData;
    [SerializeField] TileBuildData[] SetAllTileBuildData;
    public string MapFolderFilePath;
    [SerializeField] WorldTileVisual visualGrid;
    [SerializeField] Sprite TileAtlas;

    public void Awake()
    {
        allTileBuildData = SetAllTileBuildData;
        string seperator = "";
        if (Application.streamingAssetsPath.Contains("/"))
        {
            seperator = "/";
        }
        else if (Application.streamingAssetsPath.Contains("\\"))
        {
            seperator = "\\";
        }
        MapFolderFilePath = Application.streamingAssetsPath + seperator + "Maps" + seperator;
    }

    

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

    public void genNewMap(int width, int height)
    {
        buildLevels = new GridClass<WorldBuildTile>(gameObject.transform, width, height, cellSize, Vector3.zero, (int x, int y) => new WorldBuildTile(x, y, defaultTile));
        visualGrid.SetGrid(buildLevels);
    }

    public void setTile(TileBuildData data, Vector2 mousePos, int buildLevel)
    {
        buildLevels.GetXY(mousePos, out int x, out int y);
        if (!buildLevels.inBounds(x, y))
        {
            //Debug.Log("Tile Change Failed:" + x + "," + y);
            return;
        }
        Debug.Log("Tile Changed:" + x + "," + y + ":" + data.BuildingName);
        buildLevels.getGridObject(mousePos).setBuildData(data);
        buildLevels.triggerGridObjectChanged(x, y);
    }

    public void setTile(TileBuildData data, Vector2Int XY, int buildLevel)
    {
        if (!buildLevels.inBounds(XY.x, XY.y))
        {
            //Debug.Log("Tile Change Failed:" + XY.x + "," + XY.y);
            return;
        }
        Debug.Log("Tile Changed:" + XY.x + "," + XY.y + ":" + data.BuildingName);
        buildLevels.getGridObject(XY.x, XY.y).setBuildData(data);
        buildLevels.triggerGridObjectChanged(XY.x, XY.y);
    }

    public void setTile(WorldBuildTile tile, Vector2Int XY, int buildLevel)
    {
        if (!buildLevels.inBounds(XY.x, XY.y) || tile == null)
        {
            //Debug.Log("Tile Changed Fail:" + (tile == null));
            return;
        }
        // Debug.Log("Tile Changed Entire:" + XY.ToString());
        buildLevels.setGridObject(XY.x, XY.y, tile);
        buildLevels.triggerGridObjectChanged(XY.x, XY.y);
    }

    public void setTile(worldBuildTileTransferData transferData, Vector2Int XY, int buildLevel)
    {
        if (!buildLevels.inBounds(XY.x, XY.y) || transferData == null)
        {
            //Debug.Log("Tile Changed Fail:" + (transferData == null));
            return;
        }
        //Debug.Log("Tile Changed Entire:" + XY.ToString());
        buildLevels.getGridObject(XY.x, XY.y).setValuesFromTransferData(transferData);
        buildLevels.triggerGridObjectChanged(XY.x, XY.y);
    }

    public GridClass<WorldBuildTile> getBuildLayers()
    {
        return buildLevels;
    }

    //visual handler
    List<TileVisual_Script> tileVisual_Scripts = new List<TileVisual_Script>();
    public void clearVisuals()
    {
        foreach (TileVisual_Script scr in tileVisual_Scripts)
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
        setVisualGrid(0, buildLevels.width - 1, buildLevels.height - 1);
        Debug.Log("Finished Settig Visual Layering");
    }

    private void setVisualGrid(int layer, int x, int y)
    {
        if (!buildLevels.inBounds(x, y) || buildLevels.getGridObject(x, y).getDisplayOrder() != -1)
        {
            return;
        }
        //Debug.Log("Layer:" + layer + "; coords:" + x + "," + y);
        buildLevels.getGridObject(x, y).setDisplayOrder(layer);
        setVisualGrid(layer + 1, x - 1, y);
        setVisualGrid(layer + 1, x, y - 1);

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
        TileBuildData WallData;

        bool[] subGrid = new bool[9];

        public TileBuildData getMainBuildData()
        {
            if (DecorationBuildData != null)
            {
                return DecorationBuildData;
            }
            if (WallData != null)
            {
                return WallData;
            }
            return GroundBuildData;
        }
        public WorldBuildTile(int x, int y, TileBuildData defaultTileLoc)
        {
            this.x = x;
            this.y = y;
            setBuildData(defaultTileLoc);
        }

        public WorldBuildTile(worldBuildTileTransferData transferData)
        {
            x = transferData.x;
            y = transferData.y;
            displayOrder = transferData.displayOrder;
            GroundBuildData = transferData.GroundBuildData;
            DecorationBuildData = transferData.DecorationBuildData;
            WallData = transferData.WallData;
            subGrid = transferData.subGridData;
        }

        public worldBuildTileTransferData getTransferData()
        {
            setTileSubGrid();
            worldBuildTileTransferData data = new worldBuildTileTransferData();
            data.x = x;
            data.y = y;
            data.displayOrder = displayOrder;
            data.GroundBuildData = GroundBuildData;
            data.WallData = WallData;
            data.DecorationBuildData = DecorationBuildData;
            data.subGridData = subGrid;

            return data;
        }

        public void setValuesFromTransferData(worldBuildTileTransferData transferData)
        {
            x = transferData.x;
            y = transferData.y;
            displayOrder = transferData.displayOrder;
            GroundBuildData = transferData.GroundBuildData;
            DecorationBuildData = transferData.DecorationBuildData;
            WallData = transferData.WallData;
            subGrid = transferData.subGridData;
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
            switch (data.buildingType)
            {
                case TileBuildData.BuildingType.Ground:
                    this.GroundBuildData = data;
                    WallData = null;
                    break;
                case TileBuildData.BuildingType.Decoration:
                    this.DecorationBuildData = data;
                    break;
                case TileBuildData.BuildingType.Wall:
                    this.WallData = data;
                    GroundBuildData = null;
                    break;
                case TileBuildData.BuildingType.DefaultTile:
                    this.GroundBuildData = data;
                    break;
            }
            setTileSubGrid();
        }

        private void setTileSubGrid()
        {
            addSubGridBlockers(GroundBuildData);
            addSubGridBlockers(DecorationBuildData);
            addSubGridBlockers(WallData);
        }

        private void addSubGridBlockers(TileBuildData data)
        {
            if (data == null || data.BulletBlockerSubGrid == null || data.BulletBlockerSubGrid.Length == 0)
            {
                return;
            }
            bool[] adderSubGrid = data.BulletBlockerSubGrid;
            for (int i = 0; i < subGrid.Length; i++)
            {
                if (adderSubGrid[i] == true)
                {
                    subGrid[i] = true;
                }
            }
        }

        public override string ToString()
        {
            if (GroundBuildData == null)
            {
                return "null";
            }
            return GroundBuildData.BuildingName;
        }


        //pathfinding stuff
        public int gCost;
        public int hCost;
        public int fCost;
        public WorldBuildTile LastNode;

        public void calcFCost()
        {
            fCost = gCost + hCost;
        }

        public Vector2Int getXY()
        {
            return new Vector2Int(x,y);
        }

        public bool IsWalkable()
        {
            if (WallData == null)
            {
                return true;
            }
            return false;
        }
    }

    public class worldBuildTileTransferData
    {





        public int x;
        public int y;
        public int displayOrder = -1;
        //floor
        public TileBuildData GroundBuildData;

        //decoration
        public TileBuildData DecorationBuildData;

        //walls
        public TileBuildData WallData;
        public bool[] subGridData;

        public string getDataAsSaveString()
        {

            string subgridS = "";
            for (int i = 0; i < subGridData.Length; i++)
            {
                if (subGridData[i] == true)
                {
                    subgridS += "True";
                }
                else
                {
                    subgridS += "False";
                }
                if (i != subGridData.Length-1)
                {
                    subgridS += ",";
                }
            }
            string returner = "WBTTD:";
            returner += x + "," + y + "," + displayOrder + "," + getTBDName(GroundBuildData) + "," + getTBDName(DecorationBuildData) + "," +
                getTBDName(WallData) + "," + subgridS;
            return returner;
        }

        private string getTBDName(TileBuildData dat)
        {
            if (dat == null)
            {
                return "null";
            }
            else
            {
                return dat.BuildingName;
            }
        }

        public bool setDataFromSaveString(string s)
        {
            string[] tagAndData = s.Split(':');
            if (!tagAndData[0].Equals("WBTTD"))
            {
                Debug.Log("Data Read Fail: Incorrect Data Type");
                return false;
            }
            string[] data = tagAndData[1].Split(',');
            x = int.Parse(data[0]);
            y = int.Parse(data[1]);
            displayOrder = int.Parse(data[2]);
            GroundBuildData = getDataFromName(data[3]);
            DecorationBuildData = getDataFromName(data[4]);

            WallData = getDataFromName(data[5]);
            subGridData = new bool[9];
            for (int i = 9; i < data.Length; i++)
            {
                //Debug.Log("attempting to get bool values from:" + data[i] + ":" + i);
                subGridData[i-9] = bool.Parse(data[i]);
            }
            return true;

        }

        private TileBuildData getDataFromName(string name)
        {
            for (int i = 0; i < allTileBuildData.Length; i++)
            {
                if (allTileBuildData[i].BuildingName.Equals(name))
                {
                    return allTileBuildData[i];
                }
            }
            return null;
        }
    }



}
