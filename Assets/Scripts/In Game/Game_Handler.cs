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
    }

    public void spawnUnit(Vector2Int tile, int index)
    {
        Debug.Log("SpawnedUnit At:" + tile.ToString());
    }


    



    // Update is called once per frame
    void Update()
    {
        
    }
}
