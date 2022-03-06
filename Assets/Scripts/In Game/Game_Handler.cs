using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game_Handler : MonoBehaviour
{
    public static string mapFileName;
    [SerializeField] PhotonView LocalView;
    [SerializeField] SaveLoad_Handler_Script saveLoad;
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
    //game setup
    void Start()
    {
        saveLoad.load(mapFileName);
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        OtherPlayers = PhotonNetwork.PlayerListOthers;
        TotalPlayers = PhotonNetwork.PlayerList;
        AssignTeams();
        SetActiveTeam(Team.Team1);

    }

    void AssignTeams()
    {
        for (int i = 0; i < TotalPlayers.Length; i++)
        {
            LocalView.RPC("AssignLocalTeam", TotalPlayers[i], getTeamFromValue(i));
            TeamToPlayerDictionary.Add(getTeamFromValue(i), TotalPlayers[i]);
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

    
    [PunRPC] void AssignLocalTeam(Team team)
    {
        localTeam = team;
    }

    [PunRPC] void SetActiveTeam(Team activeTeam)
    {
        currentActiveTeam = activeTeam;
    }

    



    // Update is called once per frame
    void Update()
    {
        
    }
}
