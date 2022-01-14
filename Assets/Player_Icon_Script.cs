using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Player_Icon_Script : MonoBehaviour
{
    [SerializeField] TMP_Text userName;
    [SerializeField] Image colorBar;
    [SerializeField] public PhotonView photonView;
    [SerializeField] Image backGround;
    public bool ready;

   


    [PunRPC]
   public void updatePlayerNameList(string name, bool enabled, bool ready)
    {
        userName.text = name;
        if (ready)
        {
            Ready();
        }
        else
        {
            NotReady();
        }
        userName.enabled = enabled;
        colorBar.enabled = enabled;
        backGround.enabled = enabled;
    }

    public void setUserName(string name)
    {
        userName.text = name;
    }

    [PunRPC]
    public void updateReadyStatus(bool status)
    {
        //Debug.Log("updateReadyStatus was called :" + status);
        if (status)
        {
            Ready();
        }
        else
        {
            NotReady();
        }
    }

    public void Ready()
    {
        //photonView.RPC("updateReadyStatus", RpcTarget.OthersBuffered, true);
        colorBar.color = Color.green;
        ready = true;
    }

    public void NotReady()
    {
       // photonView.RPC("updateReadyStatus", RpcTarget.OthersBuffered, false);
        colorBar.color = Color.red;
        ready = false;
    }

    public void deleteMe()
    {
        Destroy(gameObject);
    }


}
