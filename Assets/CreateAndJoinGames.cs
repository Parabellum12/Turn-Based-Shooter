using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class CreateAndJoinGames : MonoBehaviourPunCallbacks
{
    public TMP_InputField GameName;
    public TMP_InputField userName;
    public TMP_Text errorText;
    float curTimer;
    [SerializeField]float maxDisapearTimer = .5f;
    [SerializeField] float maxHoldTimer = 2f;

    public void changeNick()
    {
        PhotonNetwork.NickName = userName.text;
    }



    public void createRoom()
    {
        checkNick();
        PhotonNetwork.IsMessageQueueRunning = true;
        if (checkLobbyName())
        {
            PhotonNetwork.CreateRoom(GameName.text);
        }
    }

    public void joinRoom()
    {
        checkNick();
        PhotonNetwork.IsMessageQueueRunning = true;
        if (checkLobbyName())
        {
            PhotonNetwork.JoinRoom(GameName.text); 
        }
    }

    private bool checkLobbyName()
    {
        if (GameName.text.Length == 0)
        {
            sendErrorMessage("Error", "Need Lobby Name");
            return false;
        }
        else
        {
            return true;
        }
    }

    private void checkNick()
    {
        if (PhotonNetwork.NickName.Length == 0)
        {
            PhotonNetwork.NickName = "New Player";
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Pre-Game_Lobby");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        sendErrorMessage("Failed To Create Room", message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        sendErrorMessage("Failed To Join Room", message);
    }


    private void sendErrorMessage(string reason, string errorMsg)
    {
        errorText.text = reason + "\n  Reason:" + errorMsg;
        curTimer = 0;
        errorText.alpha = 100;
        errorText.gameObject.SetActive(true);
    }

    public void back()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main Menu");
    }


    public void Update()
    {
        if (errorText.gameObject.activeSelf)
        {
            curTimer += Time.deltaTime;
            if (curTimer > maxHoldTimer)
            {
                errorText.alpha -= (100 / maxDisapearTimer);
                if (errorText.alpha <= 0)
                {
                    errorText.gameObject.SetActive(false);
                }
            }
        }
    }
}
