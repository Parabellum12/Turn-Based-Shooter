using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings_Handler_Script : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject Settings;
    [SerializeField] TMP_Text resValueTex;
    public void Start()
    {
        Settings_Handler.loadSettings();
    }


    public void resetSettings()
    {
        Settings_Handler.resetSettings();
    }


    public void toSettings()
    {
        MainMenu.SetActive(false);
        loadSettingData();
        Settings.SetActive(true);
    }

    public void toMainMenu()
    {
        MainMenu.SetActive(true);
        Settings.SetActive(false);
    }


    private void loadSettingData()
    {
        resValueTex.text = Settings_Handler.preferedRes.width + "x" + Settings_Handler.preferedRes.height;
    }


    public void nextRes()
    {
        Settings_Handler.switchResSetting(true);
        Resolution temp = Settings_Handler.getCurRes();
        resValueTex.text = temp.width + "x" + temp.height;
    }

    public void lastRes()
    {
        Settings_Handler.switchResSetting(false);
        Resolution temp = Settings_Handler.getCurRes();
        resValueTex.text = temp.width + "x" + temp.height;
    }


    public void cancel()
    {
        toMainMenu();
    }

    public void accept()
    {
        apply();
        toMainMenu();
    }

    public void apply()
    {
        Resolution tempRes = Settings_Handler.getCurRes();
        Screen.SetResolution(tempRes.width, tempRes.height, FullScreenMode.FullScreenWindow);


        PlayerPrefs.Save();
    }


}
