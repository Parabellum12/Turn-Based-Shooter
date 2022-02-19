using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unit_Icon_Script : MonoBehaviour
{
    public TMP_Text Unit_Name;
    public Image Class_Image;
    public Image selector;
    public GameObject Active;
    public GameObject Buy;

    private LobbyHandler lobbyHandler;

    public void Start()
    {
        lobbyHandler = GameObject.Find("Handler").GetComponent<LobbyHandler>();
    }
    public void callSelected()
    {
        if (Active.activeSelf)
        {
           // Debug.Log("Select This");
            lobbyHandler.UpdateSelectedUnit(this);
        }
        else if (Buy.activeSelf)
        {
           // Debug.Log("Buy Unit");
            lobbyHandler.addUnit();
            lobbyHandler.UpdateSelectedUnit(this);
        }
    }
}
