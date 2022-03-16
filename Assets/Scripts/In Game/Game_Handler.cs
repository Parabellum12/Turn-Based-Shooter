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
            Debug.Log("RPC SpawnUnits: " + i);
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
    }

    [PunRPC] public void setMap(string mapFileName)
    {
        Game_Handler.mapFileName = mapFileName.Trim();
        saveLoad.load(mapFileName);
        spawnZones = worldHandler.setupSpawnPoints(worldHandler.getBuildLayers());
    }

    [PunRPC] void spawnUnits(int spawnZoneIndex)
    {
        Debug.Log("PlayerUnits Length:" + PlayerUnits.Length + ", SpawnZoneCount:" + spawnZones.Count);
        World_Handler_Script.WorldTileSpawnPoints spawnZone = spawnZones[spawnZoneIndex];
        for (int i = 0; i < PlayerUnits.Length; i++)
        {
            spawnUnit(spawnZone.TilesPos[i % spawnZone.TilesPos.Count], i);
        }
        //LocalView.RPC("masterRecieveUnits", PhotonNetwork.MasterClient, AllUnits);
    }


    [SerializeField] GameObject UnitPrefab;
    public void spawnUnit(Vector2Int tile, int index)
    {
        Debug.Log("SpawnedUnit At:" + tile.ToString());
        Vector3 worldPosToSpawnAt = worldHandler.getBuildLayers().getWorldPosition(tile.x, tile.y) + (new Vector3(1,1) * worldHandler.cellSize) * .5f;
        GameObject unit = PhotonNetwork.Instantiate("UnitPreFab", worldPosToSpawnAt, Quaternion.identity);  //Instantiate(UnitPrefab);
        
        InGame_Unit_Handler_Script unitHandler = unit.GetComponent<InGame_Unit_Handler_Script>();
        unitHandler.setup(worldPosToSpawnAt, PlayerUnits[index]);
        AllUnits.Add(unitHandler);
    }

    //master list of units


    
    
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
        if (Input.GetMouseButtonDown(0))
        {
            handleLeftClick();
        }
        //Debug.Log("AllUnitsInGameCount:" + allUnitsInGame.Count);
    }


    void handleLeftClick()
    {
        handleUnitLeftClick();
    }

    [SerializeField] bool adjacentOnly = true;
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
            }
            else
            {
                //no unit clicked
                if (SelectedUnit != null)
                {
                    StartCoroutine(handleUnitMove());
                }
            }
        }
    }

    private IEnumerator handleUnitMove()
    {
        //unit selected and empty grid square clicked
        //do pathfinding
        worldHandler.getBuildLayers().GetXY(SelectedUnit.transform.position, out int x, out int y);
        worldHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int x2, out int y2);
        Vector2Int[] path = new Vector2Int[0];
        StartCoroutine(AstarPathing.returnPath(new Vector2Int(x, y), new Vector2Int(x2, y2), worldHandler.getBuildLayers(), adjacentOnly, (pathReturn) =>
        {
            Debug.Log("The World Is Ending");
            path = pathReturn;
        }));
        Debug.Log("The World Is Starting");
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
            string outer = "";
            foreach (Vector2Int vec in path)
            {
                outer += vec.ToString() + ",";
            }
            Debug.Log("Found Path:" + outer);
            SelectedUnit.moveToPos(path);
        }
        yield break;
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
}
