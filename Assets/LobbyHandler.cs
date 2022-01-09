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
    public TMP_Text text;

    public void Awake()
    {
        text.text = PhotonNetwork.IsMasterClient.ToString() + " Name:" + PhotonNetwork.NickName;
        Photon.Realtime.Player[] arr =  PhotonNetwork.PlayerListOthers;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " Joined");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient == true)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("JoinServer");
        }
    }

}
