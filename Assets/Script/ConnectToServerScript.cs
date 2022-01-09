using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToServerScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void Connect()
    {
        PhotonNetwork.NickName = "New Player";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("JoinServer");
    }
}
