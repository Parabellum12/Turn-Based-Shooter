using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings_Handler_Script : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject Settings;
    [SerializeField] TMP_Text resValueTex;
    [SerializeField] TMP_Text WindowModeValueTex;

    public void Start()
    {
        Settings_Handler.loadSettings();
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
        WindowModeValueTex.text = Settings_Handler.getCurWindowMode().ToString();
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

    public void nextWindowMode()
    {
        Settings_Handler.switchWindowMode(true);
        WindowModeValueTex.text = Settings_Handler.getCurWindowMode().ToString();
    }

    public void lastWindowMode()
    {
        Settings_Handler.switchWindowMode(false);
        WindowModeValueTex.text = Settings_Handler.getCurWindowMode().ToString();
    }
    public void resetSettings()
    {
        Settings_Handler.resetSettings();
        Settings_Handler.loadSettings();
        loadSettingData();
        apply();
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
        Screen.SetResolution(tempRes.width, tempRes.height, Settings_Handler.getCurWindowMode());
        //Screen.fullScreenMode = FullScreenMode.Windowed;

        PlayerPrefs.Save();
    }


}
