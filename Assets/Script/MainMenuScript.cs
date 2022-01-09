using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]GameObject mainMenu;
    [SerializeField]GameObject joinMenu;

    public void switchToJoinScreen()
    {
        mainMenu.SetActive(false);
        joinMenu.SetActive(true);
    }
     
    public void switchToTitleMenuScreen()
    {
        mainMenu.SetActive(true);
        joinMenu.SetActive(false);
    }
}
