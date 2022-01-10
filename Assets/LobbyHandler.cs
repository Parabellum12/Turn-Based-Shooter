using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    public TMP_Text UserName;
    Photon.Realtime.Player[] players;

    public void Awake()
    {
        UserName.text = " "+PhotonNetwork.NickName + "\tHost:" + PhotonNetwork.MasterClient.NickName;
        Photon.Realtime.Player[] arr =  PhotonNetwork.PlayerListOthers;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            //not host
            return;
        }
        else
        {
            //host
            players = PhotonNetwork.PlayerListOthers;
            Debug.Log(newPlayer.CustomProperties.ToString());
        }
        Debug.Log(newPlayer.NickName + " Joined");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient == true)
        {
            HostDisconnected();
        }
    }

    private void HostDisconnected()
    {
        
    }

    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("JoinServer");
    }

}
