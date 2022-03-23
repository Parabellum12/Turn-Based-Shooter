using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Game_Handler : MonoBehaviour
{
    public static string mapFileName;
    [SerializeField] PhotonView LocalView;
    [SerializeField] SaveLoad_Handler_Script saveLoad;
    [SerializeField] World_Handler_Script worldHandler;

    List<World_Handler_Script.WorldTileSpawnPoints> spawnZones;

    public static CharacterData[] PlayerUnits;
    public enum Team
    {
        Team1,
        Team2,
        Team3,
        Team4,
        Team5,
        Team6,
        Team7,
        fail
    };
    Team localTeam;
    Team currentActiveTeam;
    int currentActiveTeamIndex = 0;
    Photon.Realtime.Player[] OtherPlayers;
    Photon.Realtime.Player[] TotalPlayers;
    Dictionary<Team, Photon.Realtime.Player> TeamToPlayerDictionary = new Dictionary<Team, Photon.Realtime.Player>();
    int teamIntValue;
    //game setup
    void Start()
    {
        pathfindingVisualHandler = GameObject.FindGameObjectWithTag("PathfindingVisual").GetComponent<pathfindingColorVisualHandler>();
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        setMap(mapFileName);
        LocalView.RPC("setMap", RpcTarget.OthersBuffered, mapFileName);
        saveLoad.load(mapFileName);
        OtherPlayers = PhotonNetwork.PlayerListOthers;
        TotalPlayers = PhotonNetwork.PlayerList;
        AssignTeams();
        SetActiveTeam(Team.Team1);

        for (int i = 0; i < TotalPlayers.Length; i++)
        {
            TeamToPlayerDictionary.TryGetValue(getTeamFromValue(i), out Photon.Realtime.Player plr);
            //Debug.Log("RPC SpawnUnits: " + i);
            LocalView.RPC("spawnUnits", plr, i);
        }
    }



    [PunRPC] public void assignTeamInt(int i)
    {
        teamIntValue = i;
    }

    void AssignTeams()
    {
        for (int i = 0; i < TotalPlayers.Length; i++)
        {
            LocalView.RPC("AssignLocalTeam", TotalPlayers[i], getTeamFromValue(i));
            TeamToPlayerDictionary.Add(getTeamFromValue(i), TotalPlayers[i]);
            LocalView.RPC("assignTeamInt", TotalPlayers[i], getIntFromTeam(getTeamFromValue(i)));
        }
    }

    Team getTeamFromValue(int index)
    {
        switch (index)
        {
            case 0: return Team.Team1;
            case 1: return Team.Team2;
            case 2: return Team.Team3;
            case 3: return Team.Team4;
            case 4: return Team.Team5;
            case 5: return Team.Team6;
            case 6: return Team.Team7;
        }
        return Team.fail;
    }

    int getIntFromTeam(Team team)
    {
        switch (team)
        {
            case Team.Team1:
                return 0;
            case Team.Team2:
                return 1;
            case Team.Team3:
                return 2;
            case Team.Team4:
                return 3;
            case Team.Team5:
                return 4;
            case Team.Team6:
                return 5;
            case Team.Team7:
                return 6;
        }
        return -1;
    }

    
    [PunRPC] void AssignLocalTeam(Team team)
    {
        localTeam = team;
    }

    [PunRPC] void SetActiveTeam(Team activeTeam)
    {
        currentActiveTeam = activeTeam;
        Debug.Log("Active Team:" + activeTeam);
    }

    [PunRPC] public void setMap(string mapFileName)
    {
        Game_Handler.mapFileName = mapFileName.Trim();
        saveLoad.load(mapFileName);
        spawnZones = worldHandler.setupSpawnPoints(worldHandler.getBuildLayers());
    }

    [PunRPC] void spawnUnits(int spawnZoneIndex)
    {
        //Debug.Log("PlayerUnits Length:" + PlayerUnits.Length + ", SpawnZoneCount:" + spawnZones.Count);
        World_Handler_Script.WorldTileSpawnPoints spawnZone = spawnZones[spawnZoneIndex];
        Camera.main.transform.position = worldHandler.getBuildLayers().getWorldPosition(spawnZone.TilesPos[0].x, spawnZone.TilesPos[0].y);
        Vector2Int[] arr = new Vector2Int[PlayerUnits.Length];
        for (int i = 0; i < PlayerUnits.Length; i++)
        {
            spawnUnit(spawnZone.TilesPos[i % spawnZone.TilesPos.Count], i);
            arr[i] = spawnZone.TilesPos[i % spawnZone.TilesPos.Count];
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            LocalView.RPC("GiveMasterNewPositions", Photon.Pun.RpcTarget.MasterClient, Vector2IntArrayToString(arr), PhotonNetwork.LocalPlayer);
        }
        else
        {
            GiveMasterNewPositions(Vector2IntArrayToString(arr), PhotonNetwork.LocalPlayer);
        }
    }


    [SerializeField] GameObject UnitPrefab;
    public void spawnUnit(Vector2Int tile, int index)
    {
        //Debug.Log("SpawnedUnit At:" + tile.ToString());
        Vector3 worldPosToSpawnAt = worldHandler.getBuildLayers().getWorldPosition(tile.x, tile.y) + (new Vector3(1,1) * worldHandler.cellSize) * .5f;
        GameObject unit = PhotonNetwork.Instantiate("UnitPreFab", worldPosToSpawnAt, Quaternion.identity);  //Instantiate(UnitPrefab);
        
        InGame_Unit_Handler_Script unitHandler = unit.GetComponent<InGame_Unit_Handler_Script>();
        unitHandler.setup(worldPosToSpawnAt, PlayerUnits[index], tile);
        AllUnits.Add(unitHandler);
    }

    //turn handleing


    [PunRPC] void MasterNextTurn()
    {
        currentActiveTeamIndex = (currentActiveTeamIndex + 1) % TotalPlayers.Length;
        LocalView.RPC("recieveNextTurn", RpcTarget.All, currentActiveTeamIndex);
    }

    [PunRPC] void recieveNextTurn(int index)
    {
        currentActiveTeamIndex = index;
        currentActiveTeam = getTeamFromValue(currentActiveTeamIndex);
        Debug.Log("Next Turn Set To:" + currentActiveTeam);
    }

    public void EndMyTurn()
    {
        CancelMoveRequest = true;
        if (IsMyTurn())
        {
            LocalView.RPC("MasterNextTurn", RpcTarget.MasterClient);
        }
    }




    //gameStateHandling


    public bool IsMyTurn()
    {
        if (currentActiveTeam == localTeam)
        {
            return true;
        }
        return false;
    }




    public InGame_Unit_Handler_Script SelectedUnit;
    //local units
    public List<InGame_Unit_Handler_Script> AllUnits = new List<InGame_Unit_Handler_Script>();


    // Update is called once per frame
    void Update()
    {
        if (IsMyTurn())
        {


            if (Input.GetMouseButtonDown(0) && !UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
            {
                handleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1) && !UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
            {
                handleRightClick();
            }
            //Debug.Log("AllUnitsInGameCount:" + allUnitsInGame.Count);
        }
    }


    void handleRightClick()
    {
        handleUnitRightClick();
    }

    void handleLeftClick()
    {
        handleUnitLeftClick();
    }


    void handleUnitRightClick()
    {
        SelectedUnit = null;
        if (WaitingForAcceptionOfPath)
        {
            Debug.Log("cancelMoveRequest");
            CancelMoveRequest = true;
            resetPathVisualGrid();
        }
    }

    [SerializeField] bool adjacentOnly = false;
    void handleUnitLeftClick()
    {
        if (IsMouseOverUI())
        {
            handleUILeftClick();
        }
        else
        {
            InGame_Unit_Handler_Script tempSelect = null;
            foreach (InGame_Unit_Handler_Script scr in AllUnits)
            {
                if (scr.mouseOver)
                {
                    tempSelect = scr;
                }
            }

            if (tempSelect != null)
            {
                //unit clicked
                SelectedUnit = tempSelect;
                resetPathVisualGrid();
            }
            else
            {
                //no unit clicked
                if (SelectedUnit != null && !moving)
                {
                    if (!WaitingForAcceptionOfPath)
                    {
                        //new path
                        StartCoroutine(handleUnitMove());
                        WaitingForAcceptionOfPath = true;
                    }
                    else
                    {
                        //accept path
                        AcceptedMovePath = true;
                    }
                }
            }
        }
    }



    private void resetPathVisualGrid()
    {

        for (int i = 0; i < worldHandler.getBuildLayers().width; i++)
        {
            for (int j = 0; j < worldHandler.getBuildLayers().height; j++)
            {
                worldHandler.getBuildLayers().getGridObject(i, j).setPathfindingData(null);
            }
        }
        Debug.Log("kill old path");
        pathfindingVisualHandler.SetGrid(worldHandler.getBuildLayers());
    }


    bool moving = false;
    bool AcceptedMovePath = false;
    bool CancelMoveRequest = false;
    bool WaitingForAcceptionOfPath = false;

    [SerializeField] int greenColorDist = 80;
    [SerializeField] int orangeColorDist = 160;
    [SerializeField] int RedColorDist = 240;
    private int calculateUnitMoveColors(World_Handler_Script.WorldBuildTile tile)
    {
        if (tile.gCost < greenColorDist)
        {
            return 2;
        }
        else if (tile.gCost < orangeColorDist)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }


    private IEnumerator handleUnitMove()
    {
        Debug.Log("start new path");

        //reset path visuals
        resetPathVisualGrid();



        //unit selected and empty grid square clicked
        //do pathfinding

        worldHandler.getBuildLayers().GetXY(SelectedUnit.transform.position, out int x, out int y);
        worldHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int x2, out int y2);

        if (clickedPosOccupied(new Vector2Int(x2, y2)))
        {
            //Debug.Log("spot already used");
            yield break;
        }


        Vector2Int[] path = new Vector2Int[0];
        yield return StartCoroutine(AstarPathing.returnPath(new Vector2Int(x, y), new Vector2Int(x2, y2), worldHandler.getBuildLayers(), adjacentOnly, getRestrictedTiles(), (pathReturn) =>
        {
            //Debug.Log("The World Is Ending");
            path = pathReturn;
        }));
        if (path != null && path.Length > 0)
        {
            foreach (Vector2Int vec in path)
            {
                worldHandler.getBuildLayers().getGridObject(vec.x, vec.y).setPathfindingData(pathfindingColors[calculateUnitMoveColors(worldHandler.getBuildLayers().getGridObject(vec.x, vec.y))]);
            }
            pathfindingVisualHandler.SetGrid(worldHandler.getBuildLayers());
        }


        //Debug.Log("The World Is Starting");
        if (path == null)
        {
            Debug.Log("invalid path null");
        }
        else if (path.Length == 0)
        {

            Debug.Log("invalid path no length");
        }
        else
        {

            //wait for acceptance of path
            while (!AcceptedMovePath)
            {
                if (CancelMoveRequest)
                {
                    AcceptedMovePath = false;
                    CancelMoveRequest = false;
                    WaitingForAcceptionOfPath = false;

                    resetPathVisualGrid();

                    yield break;
                }
                yield return null;
            }
            worldHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int xnewTest, out int ynewTest);
            if (!(xnewTest == x2 && ynewTest == y2))
            {
                //not clicked on same pos

                
                AcceptedMovePath = false;
                StartCoroutine(handleUnitMove());
                yield break;
            }
            moving = true;
            AcceptedMovePath = false;
            WaitingForAcceptionOfPath = false;


            string outer = "";
            foreach (Vector2Int vec in path)
            {
                outer += vec.ToString() + ",";
            }
            //Debug.Log("Found Path:" + outer);
            yield return StartCoroutine(SelectedUnit.moveToPos(path));
            SelectedUnit = null;

            Vector2Int[] arr = new Vector2Int[AllUnits.ToArray().Length];
            int index = 0;
            foreach (InGame_Unit_Handler_Script scr in AllUnits)
            {
                arr[index] = scr.getGridPos();
                index++;
            }
            LocalView.RPC("GiveMasterNewPositions", Photon.Pun.RpcTarget.MasterClient, Vector2IntArrayToString(arr), PhotonNetwork.LocalPlayer);


        }
        moving = false;
        if (path != null && path.Length != 0)
        {
            foreach (Vector2Int vec in path)
            {
                worldHandler.getBuildLayers().getGridObject(vec.x, vec.y).setPathfindingData(null);
            }
            pathfindingVisualHandler.SetGrid(worldHandler.getBuildLayers());
        }
        yield break;
    }

    [SerializeField] TileBuildData[] pathfindingColors = new TileBuildData[3];
    [SerializeField] pathfindingColorVisualHandler pathfindingVisualHandler;



    public List<Vector2Int> getRestrictedTiles()
    {
        List<Vector2Int> returner = new List<Vector2Int>();
        int count = 0;
        foreach (Vector2Int[] arr in playerToUnitDictionary.Values)
        {
            foreach (Vector2Int Arrpos in arr)
            {
                returner.Add(Arrpos);
                count++;
            }
        }
        //Debug.Log("WHYYYYYYY:"+count);
        return returner;
    }

    public bool clickedPosOccupied(Vector2Int pos)
    {
        foreach (Vector2Int[] arr in playerToUnitDictionary.Values)
        {
            foreach (Vector2Int Arrpos in arr)
            {
                if (Arrpos.Equals(pos))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void handleUILeftClick()
    {

    }

    bool IsMouseOverUI()
    {
        return false;
    }


    public Vector3 getPosOnGrid(Vector2Int pos)
    {
        Vector3 newPos = worldHandler.getBuildLayers().getWorldPosition(pos.x, pos.y);
        newPos.x = newPos.x + worldHandler.getBuildLayers().getCellSize() / 2;
        newPos.y = newPos.y + worldHandler.getBuildLayers().getCellSize() / 2;
        return newPos;
    }

    



    //network unit pos storage handling
    Dictionary<Photon.Realtime.Player, Vector2Int[]> playerToUnitDictionary = new Dictionary<Photon.Realtime.Player, Vector2Int[]>();
    [PunRPC] void GiveMasterNewPositions(string arrAsString, Photon.Realtime.Player plr)
    {
        playerToUnitDictionary.Remove(plr);
        playerToUnitDictionary.Add(plr, stringToVector2Int(arrAsString));
        LocalView.RPC("giveClientPlayerPositions", Photon.Pun.RpcTarget.Others, arrAsString, plr);
    }

    [PunRPC] void clientSetDictionary(Dictionary<Photon.Realtime.Player, Vector2Int[]> playerToUnitDictionary)
    {
        this.playerToUnitDictionary = playerToUnitDictionary;
    }

    [PunRPC] void clientClearDictionary()
    {
        playerToUnitDictionary.Clear();
    }

    [PunRPC] void giveClientPlayerPositions(string asString, Photon.Realtime.Player plr)
    {
        playerToUnitDictionary.Remove(plr);
        playerToUnitDictionary.Add(plr, stringToVector2Int(asString));
    }

    private string Vector2IntArrayToString(Vector2Int[] arr)
    {
        string returner = "";
        for (int i = 0; i < arr.Length; i++)
        {
            returner += arr[i].x + "," + arr[i].y;
            if (i != arr.Length-1)
            {
                returner += ";";
            }
        }

        //Debug.Log("Vector2IntArrayToString returner:" + returner);
        return returner;
    }

    private Vector2Int[] stringToVector2Int(string s)
    {
        string[] values = s.Split(';');
        //Debug.Log("values length:" + values.Length);
        Vector2Int[] returner = new Vector2Int[values.Length];
        int index = 0;
        foreach (string str in values)
        {
            string[] data = str.Split(',');
            //Debug.Log("vectorarr data Length:"+data.Length);
            returner[index] = new Vector2Int(int.Parse(data[0]), int.Parse(data[1]));

            index++;
        }
        return returner;
    }
}
