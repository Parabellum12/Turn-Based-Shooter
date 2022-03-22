using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public class World_Handler_Script : MonoBehaviour
{
    GridClass<WorldBuildTile> buildLevels = null;
    [SerializeField] public float cellSize = 15f;
    [SerializeField] TMP_Dropdown mapSize;
    [SerializeField] int baseWidth = 60;
    [SerializeField] int baseHeight = 60;
    [SerializeField] TileBuildData defaultTile;
    public static TileBuildData[] allTileBuildData;
    [SerializeField] TileBuildData[] SetAllTileBuildData;
    public string MapFolderFilePath;
    [SerializeField] WorldTileVisual visualGrid;
    [SerializeField] Sprite TileAtlas;

    [SerializeField] bool setDebugTo = false;
    public static bool debugging = false;

    List<WorldTileSpawnPoints> spawnPoints = new List<WorldTileSpawnPoints>();

    public void Awake()
    {
        debugging = setDebugTo;
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
        setupSpawnPoints(buildLevels);

    }

    public void testspawnPointSetup()
    {
        string output = "";
        foreach (WorldTileSpawnPoints spawn in setupSpawnPoints(buildLevels))
        {
            output += " (";
            foreach (Vector2Int vec in spawn.TilesPos)
            {
                output += vec.ToString() + ",";
            }
            output += "); ";
        }
        Debug.Log(output);
    }
    
    public List<WorldTileSpawnPoints> setupSpawnPoints(GridClass<WorldBuildTile> gridmap)
    {
        //Debug.Log("WHYYYYYYYY");
        List<WorldTileSpawnPoints> returner = new List<WorldTileSpawnPoints>();
        List<Vector2Int> openList = new List<Vector2Int>();
        List<Vector2Int> closedList = new List<Vector2Int>();
        for (int i = 0; i < gridmap.width; i++)
        {
            for (int j = 0; j < gridmap.height; j++)
            {
                //Debug.Log(i + "," + j);
                if (gridmap.getGridObject(i,j).isSpawnPoint())
                {
                    openList.Add(new Vector2Int(i,j));
                    gridmap.getGridObject(i, j).openForSpawnpointTesting = true;
                }
            }
        }


        while (openList.Count > 0)
        {
            List<Vector2Int> toAdd = spawnpointTesting(buildLevels.getGridObject(openList[0].x, openList[0].y), buildLevels);
            foreach (Vector2Int vec in toAdd)
            {
                openList.Remove(vec);
                closedList.Add(vec);
            }
            WorldTileSpawnPoints spawnPoint = new WorldTileSpawnPoints();
            spawnPoint.TilesPos = toAdd;
            returner.Add(spawnPoint);
        }
        //Debug.Log("SpawnZone Count:"+returner.Count);
        return returner;
    }

    private List<Vector2Int> spawnpointTesting(WorldBuildTile node, GridClass<WorldBuildTile> tileGrid)
    {
        
        List<Vector2Int> posList = new List<Vector2Int>();
        if (!node.openForSpawnpointTesting)
        {
            return posList;
        }
        if (node.isSpawnPoint())
        {
            posList.Add(node.getXY());
            node.openForSpawnpointTesting = false;
        }
        else
        {
            return posList;
        }

        Vector2Int tile = new Vector2Int(node.getXY().x + 1, node.getXY().y);
        if (tileGrid.inBounds(tile.x, tile.y))
        {
            List<Vector2Int> temp = spawnpointTesting(tileGrid.getGridObject(tile.x, tile.y), tileGrid);
            foreach (Vector2Int vec in temp)
            {
                posList.Add(vec);
            }
        }

        tile = new Vector2Int(node.getXY().x - 1, node.getXY().y);
        if (tileGrid.inBounds(tile.x, tile.y))
        {
            List<Vector2Int> temp = spawnpointTesting(tileGrid.getGridObject(tile.x, tile.y), tileGrid);
            foreach (Vector2Int vec in temp)
            {
                posList.Add(vec);
            }
        }

        tile = new Vector2Int(node.getXY().x, node.getXY().y - 1);
        if (tileGrid.inBounds(tile.x, tile.y))
        {
            List<Vector2Int> temp = spawnpointTesting(tileGrid.getGridObject(tile.x, tile.y), tileGrid);
            foreach (Vector2Int vec in temp)
            {
                posList.Add(vec);
            }
        }

        tile = new Vector2Int(node.getXY().x, node.getXY().y + 1);
        if (tileGrid.inBounds(tile.x, tile.y))
        {
            List<Vector2Int> temp = spawnpointTesting(tileGrid.getGridObject(tile.x, tile.y), tileGrid);
            foreach (Vector2Int vec in temp)
            {
                posList.Add(vec);
            }
        }


        return posList;
    }

    public void genNewMap(int width, int height)
    {
        buildLevels = new GridClass<WorldBuildTile>(gameObject.transform, width, height, cellSize, Vector3.zero, (int x, int y) => new WorldBuildTile(x, y, defaultTile));
        visualGrid.SetGrid(buildLevels);


        /* A* preview
        AstarPathing pathing = new AstarPathing();
        Vector2Int[] path = pathing.returnPath(new Vector2Int(0,0), new Vector2Int(5, 10), buildLevels);
        string output = "";
        foreach (Vector2Int vec in path)
        {
            output += "(" + vec.x + "," + vec.y +"), ";
            buildLevels.getGridObject(vec.x, vec.y).setBuildData(allTileBuildData[0]);
            buildLevels.triggerGridObjectChanged(vec.x, vec.y);
        }
        Debug.Log(output);
        */
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

        TileBuildData pathfindingData = null;

        bool[] subGrid = new bool[9];

        public TileBuildData getMainBuildData()
        {
            if (DecorationBuildData != null && World_Handler_Script.debugging)
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




        public void setPathfindingData(TileBuildData color)
        {
            pathfindingData = color;
        }

        public TileBuildData getPathfindingColors()
        {
            return pathfindingData;
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
                    this.DecorationBuildData = null;
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
        public void erase()
        {
            GroundBuildData = null;
            DecorationBuildData = null;
            WallData = null;
        }


        public bool hasUnitOnIt;

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
            if (WallData == null && !hasUnitOnIt)
            {
                return true;
            }
            return false;
        }


        //spawnpoint stuff


        public bool isSpawnPoint()
        {
            if (DecorationBuildData == allTileBuildData[10])
            {
                return true;
            }
            return false;
        }

        public bool openForSpawnpointTesting = true;
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


    public class WorldTileSpawnPoints
    {
        public List<Vector2Int> TilesPos;
    }



}
