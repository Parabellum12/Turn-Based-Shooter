using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    public TMP_Text UserName;
    Photon.Realtime.Player[] players;
    [SerializeField] GameObject popupPrefab;
    [SerializeField] Canvas canvas;
    [SerializeField] Button playButton;
    [SerializeField] Player_Icon_Script[] playerIcons;
    [SerializeField] PhotonView localView;

    //unit list icons
    bool[] playerIconsused;
    [SerializeField] CharacterData[] units = new CharacterData[5];
    [SerializeField] Unit_Icon_Script[] unitIcons = new Unit_Icon_Script[8];
    [SerializeField] private int Currently_Selected_Icon = 0;

    //unit managment
    [SerializeField] TMP_Text openClose_UnitManagment_Text;
    [SerializeField] GameObject UnitManagment;
    [SerializeField] Unit_Stats_Handler_Script unit_Stats_Handler_Script;

    //map selection
    [SerializeField] TMP_Dropdown mapSelector;
    string fileSystemSeperator;



    public bool debugging = false;
    [SerializeField] bool autoStart = false;


    bool isUnitManagementOpen = false;
    public void Start()
    {
        if (autoStart)
        {
            debugging = true;
        }
        if (Application.streamingAssetsPath.Contains("/"))
        {
            fileSystemSeperator = "/";
        }
        else if (Application.streamingAssetsPath.Contains("\\"))
        {
            fileSystemSeperator = "\\";
        }




        UserName.text = " User:"+PhotonNetwork.NickName + "\tHost:" + PhotonNetwork.MasterClient.NickName + " "; 
        //Debug.Log(" User:" + PhotonNetwork.NickName + "\tHost:" + PhotonNetwork.MasterClient.NickName + " ");
        //Debug.Log("hello?");
        players =  PhotonNetwork.PlayerList;
        if (!PhotonNetwork.IsMasterClient)
        {
            mapSelector.gameObject.SetActive(false);
            setToReadyButton();
            for (int i = 0; i < playerIcons.Length; i++)
            {
                playerIcons[i].updatePlayerNameList("UserName", false, false);
            }
        }
        else
        {
            //playerIcons = null;
            //is host
            mapSelector.gameObject.SetActive(true);
            updattMapFileList();
            playButton.onClick.AddListener(play);
            updatePlayerList();
        }
        updateUnitCount();
        closeUnitManagment();
        UnitManagment.GetComponent<Unit_Stats_Handler_Script>().switchToLoadout();
        PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.AutomaticallySyncScene = true;

        if (autoStart)
        {
            play();
        }

        handlePersistantData();
    }

    void handlePersistantData()
    {
        List<CharacterData> characterDatas = SessionPersistantDataHandler.getPersistantUnitData();
        if (characterDatas != null && characterDatas.Count > 0)
        {
            units = characterDatas.ToArray();
            updateUnitCount();
        }
    }

    public void openAndCloseUnitManagement()
    {
        if (isUnitManagementOpen)
        {
            //close
            closeUnitManagment();
        }
        else
        {
            //open
            openUnitManagment();
            unit_Stats_Handler_Script.getStats(units[Currently_Selected_Icon]);
        }
    }

    private void closeUnitManagment()
    {
        isUnitManagementOpen = false;
        openClose_UnitManagment_Text.text = "Open Unit Managment";
        UnitManagment.SetActive(false);
    }

    private void openUnitManagment()
    {
        isUnitManagementOpen = true;
        openClose_UnitManagment_Text.text = "Close Unit Managment";
        UnitManagment.SetActive(true);
    }



    public void addUnit()
    {
        CharacterData[] temp = new CharacterData[units.Length+1];
        for (int i = 0; i < units.Length; i++)
        {
            temp[i] = units[i];
        }
        temp[temp.Length-1] = new CharacterData(true);
        units = temp;
        //Debug.Log("whyyyyy :" + temp.Length + ";" + units.Length);
        updateUnitCount();
    }


    public void updateUnitCount()
    {
        if (Currently_Selected_Icon > units.Length)
        {
            Currently_Selected_Icon = 0;
        }
        for (int i = 0; i < unitIcons.Length; i++)
        {
            
            if (i < units.Length)
            {
                if (units[i] == null)
                {
                    units[i] = new CharacterData(true);
                }
                unitIcons[i].Unit_Name.text = units[i].CharacterName;
                unitIcons[i].Unit_Name.enableAutoSizing = false;
                unitIcons[i].Unit_Name.enableAutoSizing = true;
                if (i == Currently_Selected_Icon)
                {
                    unitIcons[i].selector.enabled = true;
                }
                else
                {
                    unitIcons[i].selector.enabled = false;
                }
                unitIcons[i].Class_Image.sprite = units[i].ClassImage;
                unitIcons[i].Active.SetActive(true);
                unitIcons[i].Buy.SetActive(false);
            }
            else
            {
                unitIcons[i].gameObject.SetActive(false);
            }
        }
        if (units.Length < unitIcons.Length)
        {
            unitIcons[units.Length].gameObject.SetActive(true);
            unitIcons[units.Length].Active.SetActive(false);
            unitIcons[units.Length].Buy.SetActive(true);
        }
    }

    public void UpdateSelectedUnit(Unit_Icon_Script caller)
    {
        for (int i = 0; i < unitIcons.Length; i++)
        {
            if (unitIcons[i].Equals(caller))
            {
                unitIcons[i].selector.enabled = true;
                Currently_Selected_Icon = i;
                unit_Stats_Handler_Script.getStats(units[i]);
            }
            else
            {
                unitIcons[i].selector.enabled = false;
            }
        }
    }

    private void setToReadyButton()
    {
        playButton.onClick.RemoveListener(play);
        playButton.onClick.AddListener(readyUpdate);
        playButton.GetComponentInChildren<TMP_Text>().text = "Not Ready";
    }

    public void readyUpdate()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            TMP_Text text = playButton.GetComponentInChildren<TMP_Text>();
            if (text.text.Equals("Ready"))
            {
                text.text = "Not Ready";

            }
            else
            {
                text.text = "Ready";
            }
            if (text.text.Equals("Ready"))
            {
                //stop unit custimization
                closeUnitManagment();
                UnitManagment.GetComponent<Button>().interactable = false;
            }
            else
            {
                //allow unit custimization
                UnitManagment.GetComponent<Button>().interactable = true;
            }
            localView.RPC("handleReadyUp", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        }
    }


    [PunRPC]
    public void handleReadyUp(Photon.Realtime.Player plr)
    {
        SessionPersistantDataHandler.setPersistantUnitSquad(units);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Equals(plr))
            {
                PhotonView iconView = playerIcons[i].photonView;
                iconView.RPC("updateReadyStatus", RpcTarget.OthersBuffered, !playerIcons[i].ready);
                playerIcons[i].updateReadyStatus(!playerIcons[i].ready);

                


                return;
            }
        }
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
            Debug.Log("host calls player entered room");
            players = PhotonNetwork.PlayerList;
            updatePlayerList();
        }
        //Debug.Log(newPlayer.NickName + " Joined");
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
        //Debug.Log(otherPlayer.NickName + " left");
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
        if (allPlayersReady())
        {
            Game_Handler.mapFileName = getSelectedMap();
            localView.RPC("UnitHandoff", RpcTarget.Others);
            UnitHandoff();
            PhotonNetwork.LoadLevel("InGame");
        }
    }







    private string getSelectedMap()
    {
        string mapName = FilDropDown_ValueToFileName[mapSelector.value];
        Debug.Log("SelectedMap:" + mapName);

        return mapName;
    }

    [PunRPC]
    private void UnitHandoff()
    {
        Game_Handler.PlayerUnits = units;
    }

    private bool allPlayersReady()
    {
        int amount = 0;
        for (int i = 0; i < playerIcons.Length; i++)
        {
            Player_Icon_Script temp = playerIcons[i].GetComponent<Player_Icon_Script>();
            if (temp.gameObject.activeSelf)
            {
                amount++;
                if (!temp.ready)
                {
                    //Debug.Log("Error: Someone Not Ready:" + players[i].NickName);
                    return false;
                }
            }
        }
        if (amount < 2 && !debugging)
        {
            return false;
        }
        return true;
    }


    


    public void updatePlayerList()
    {
       // Debug.Log("gotcha");
        if(playerIcons != null)
        {
            foreach (Player_Icon_Script g in playerIcons)
            {
                g.gameObject.SetActive(false);
                PhotonView view = g.photonView;
                view.RPC("updatePlayerNameList", RpcTarget.Others, "UserName", false, false);
            }
        }
        playerIconsused = new bool[players.Length];
        int index = 0;
        foreach (Photon.Realtime.Player plr in players)
        {
            //get next avalible icon
            for (int i = 0; i < playerIconsused.Length; i++)
            {
                if (playerIconsused[i] == false)
                {
                    index = i;
                    playerIconsused[i] = true;
                    break;
                }
            }

            //set set icon data
            playerIcons[index].setUserName(plr.NickName);
            PhotonView view = playerIcons[index].photonView;
            if (!plr.IsMasterClient)
            {
                playerIcons[index].updatePlayerNameList(plr.NickName, true, false);
                view.RPC("updatePlayerNameList", RpcTarget.Others, plr.NickName, true, false);
            }
            else
            {
                playerIcons[index].updatePlayerNameList(plr.NickName, true, true);
                view.RPC("updatePlayerNameList", RpcTarget.Others, plr.NickName, true, true);
            }
            playerIcons[index].gameObject.SetActive(true);
        }



    }






    [SerializeField] Dictionary<int, string> FilDropDown_ValueToFileName = new Dictionary<int, string>();
    private void updattMapFileList()
    {
        mapSelector.options.Clear();
        //LoadFileDropdown.ClearOptions();
        FilDropDown_ValueToFileName.Clear();
        string[] files = System.IO.Directory.GetFiles(Application.streamingAssetsPath + fileSystemSeperator + "Maps", "*.MapData");
        //Debug.Log(Application.streamingAssetsPath + fileSystemSeperator + "Maps");
        //Debug.Log("Files: Length:" + files.Length);
        //TMP_Dropdown.OptionData customOption = new TMP_Dropdown.OptionData("New File");
        //mapSelector.options.Add(customOption);
        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log(files[i]);
            string name2 = "";
            StreamReader sr = new StreamReader(files[i]);
            name2 = sr.ReadLine().Split(',')[2];

            string fileNameS = "";
            char[] temp = files[i].ToCharArray();
            bool fileName = false;
            for (int j = temp.Length - 1; j >= 0; j--)
            {
                if (temp[j] == '/' || temp[j] == '\\')
                {
                    break;
                }
                if (fileName)
                {
                    fileNameS = temp[j] + fileNameS;
                }
                else
                {
                    if (temp[j] == '.')
                    {
                        fileName = true;
                    }
                }
            }
            FilDropDown_ValueToFileName.Add(i, fileNameS);


            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(name2);
            mapSelector.options.Add(optionData);
        }
    }



}
