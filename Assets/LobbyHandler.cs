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
    [SerializeField] Image playerListCanvas;
    Player_Icon_Script[] playerIcons;

    public void Awake()
    {
        UserName.text = " User:"+PhotonNetwork.NickName + "\tHost:" + PhotonNetwork.MasterClient.NickName + " ";
        players =  PhotonNetwork.PlayerList;
        if (!PhotonNetwork.IsMasterClient)
        {
            playButton.interactable = false;
        }
        else
        {
            playerIcons = null;
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
                g.deleteMe();
            }
        }
        playerIcons = new Player_Icon_Script[players.Length];
        int index = 0;
        int x = 0;
        int y= 0;
        foreach (Photon.Realtime.Player plr in players)
        {
            GameObject temp = PhotonNetwork.Instantiate("plr_Icon", getIconOffset(60, playerListCanvas, index), Quaternion.identity);
            temp.transform.SetParent(playerListCanvas.transform); 
            RectTransform rectTrans = temp.GetComponent<RectTransform>();
            rectTrans.SetParent(playerListCanvas.transform);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 170f);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);
            Player_Icon_Script iconS = temp.GetComponent<Player_Icon_Script>();
            iconS.setUserName(plr.NickName);
            iconS.NotReady();
            iconS.updateColorBarSize();
            playerIcons[index] = iconS;

            index++;
        }
    }

    private Vector3 getIconOffset(float iconheight, Image backGround, int index)
    {
        Vector3 returner = backGround.rectTransform.position;
        Debug.Log(returner.y + ":" + (backGround.rectTransform.rect.height) + ":" + (backGround.rectTransform.rect.height / 2f));
        float offsetAmount = (215) - (index * iconheight);
        returner.y += offsetAmount;
        return returner;
    }




}
