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
    [SerializeField] GameObject popupPrefab;
    [SerializeField] Canvas canvas;
    [SerializeField] Button playButton;

    public void Awake()
    {
        UserName.text = " User:"+PhotonNetwork.NickName + "\tHost:" + PhotonNetwork.MasterClient.NickName + " ";
        Photon.Realtime.Player[] arr =  PhotonNetwork.PlayerListOthers;
        if (!PhotonNetwork.IsMasterClient)
        {
            playButton.onClick = null;
            playButton.interactable = false;
        }

        PhotonNetwork.AutomaticallySyncScene = true;
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
        }
        Debug.Log(newPlayer.NickName + " Joined");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
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
        }
        Debug.Log(otherPlayer.NickName + " left");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        HostDisconnected();
    }

    private void HostDisconnected()
    {
        Debug.Log("HostDisconnected");
        GameObject popup = Instantiate(popupPrefab, canvas.transform.position, Quaternion.identity);
        popup.transform.SetParent(canvas.transform); 
        popupScript popScript = popup.GetComponent<popupScript>();
        popScript.text.text = "Host Disconnected";
        popScript.buttonText.text = "Ok";
        popScript.button.onClick.AddListener(Disconnect);
    }

    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("JoinServer");
    }



    public void play()
    {
        PhotonNetwork.LoadLevel("InGame");
    }







}
