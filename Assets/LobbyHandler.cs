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
    [SerializeField] Player_Icon_Script[] playerIcons;
    bool[] playerIconsused;

    public void start()
    {
        UserName.text = " User:"+PhotonNetwork.NickName + "\tHost:" + PhotonNetwork.MasterClient.NickName + " ";
        Debug.Log(" User:" + PhotonNetwork.NickName + "\tHost:" + PhotonNetwork.MasterClient.NickName + " ");
        Debug.Log("hello?");
        players =  PhotonNetwork.PlayerList;
        if (!PhotonNetwork.IsMasterClient)
        {
            playButton.interactable = false;
        }
        else
        {
            //playerIcons = null;
            //is host
            updatePlayerList();
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
            players = PhotonNetwork.PlayerList;
            updatePlayerList();
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
            players = PhotonNetwork.PlayerList;
            updatePlayerList();
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
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("JoinServer");
    }



    public void play()
    {
        PhotonNetwork.LoadLevel("InGame");
    }


    public void updatePlayerList()
    {
        Debug.Log("gotcha");
        if(playerIcons != null)
        {
            foreach (Player_Icon_Script g in playerIcons)
            {
                g.gameObject.SetActive(false);
            }
        }
        playerIconsused = new bool[players.Length];
        int index = 0;
        foreach (Photon.Realtime.Player plr in players)
        {
            for (int i = 0; i < playerIconsused.Length; i++)
            {
                if (playerIconsused[i] == false)
                {
                    index = i;
                    playerIconsused[i] = true;
                    break;
                }
            }

            playerIcons[index].setUserName(plr.NickName);
            playerIcons[index].NotReady();
            playerIcons[index].gameObject.SetActive(true);
        }
    }

    




}
