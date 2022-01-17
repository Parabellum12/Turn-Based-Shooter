using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class ConnectToServerScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text loadingText;
    private bool loading = false;
    float time = 0;
    int numDots = 0;


    private void Awake()
    {
        loadingText.gameObject.SetActive(false);
    }

    public void Connect()
    {
        PhotonNetwork.NickName = "New Player";
        loading = true;
        loadingText.gameObject.SetActive(true);
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


    public void exit()
    {
        Application.Quit();
    }

    public void Update()
    {
        if (loading)
        {
            time += Time.deltaTime;
            if (time > 1)
            {
                time -= 1;
                numDots++;
                numDots = (numDots % 4);
                loadingText.text = "Connecting" + getDots();
                //Debug.Log(numDots);
            }
        }
    }

    private string getDots()
    {
        string dots = "";
        for (int i = 0; i < numDots; i++)
        {
            dots += ".";
        }
        return dots;
    }

}
