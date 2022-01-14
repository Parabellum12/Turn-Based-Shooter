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
    public int index = 0;


    public void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            //Destroy(gameObject);
        }
    }

    public void setUserName(string name)
    {
        userName.text = name;
    }

    public void Ready()
    {
        colorBar.color = Color.green;
    }

    public void NotReady()
    {
        colorBar.color = Color.red;
    }

    public void deleteMe()
    {
        Destroy(gameObject);
    }


}
