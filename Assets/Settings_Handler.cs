using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings_Handler
{
    public static Resolution[] resolutions;
    private static int resIndex = 0;
    public static Resolution preferedRes;




    static string errorFlags = "0";
    static Dictionary<int, string> errorIndexToKey = new Dictionary<int, string>()
    {
        {0, "Resolution"}
    };
    /*resolution
     * 
     * 
     * 
     */

    


    public static void loadSettings()
    {
        resetFlags();
        loadResolutionSettings();




        checkAndFixErrors();
        setPreferences();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Debug.Log(resolutions[i].ToString());
        }
    }


    private static void checkAndFixErrors()
    {
        char[] error_flags = errorFlags.ToCharArray();
        bool errorFix = false;
        for (int i = 0; i < error_flags.Length; i++)
        {
            Debug.Log("test");
            if (error_flags[i] == '1')
            {
                errorFix = true;
                errorIndexToKey.TryGetValue(i, out string value);
                switch (value)
                {
                    case "Resolution":
                        fixResolution();
                        loadResolutionSettings();
                        break;
                };

            }
        }
        if (errorFix)
        {
            PlayerPrefs.Save();
        }
    }

    private static void fixResolution()
    {
        Debug.Log("ResFix");
        Resolution[] arr = Screen.resolutions;
        PlayerPrefs.SetInt("ResolutionNumber", arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            string temp = arr[i].width + ":" + arr[i].height;
            PlayerPrefs.SetString("Resolution" + i, temp);
        }

        string pref = "1920:1080";
        PlayerPrefs.SetString("ResolutionPref", pref);
    }

    private static void setPreferences()
    {

    }





    private static void loadResolutionSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionNumber"))
        {
            setErrorFlag(0, false);
            Debug.Log("How");

            string[] temp2 = PlayerPrefs.GetString("ResolutionPref").Split(':');
            Resolution tempRes2 = new Resolution();
            tempRes2.width = int.Parse(temp2[0]);
            tempRes2.height = int.Parse(temp2[1]);
            tempRes2.refreshRate = 60;
            preferedRes = tempRes2;


            resolutions = new Resolution[PlayerPrefs.GetInt("ResolutionNumber")];
            for (int i = 0; i < resolutions.Length; i++)
            {
                string[] temp1 = PlayerPrefs.GetString("Resolution" + i).Split(':');
                Resolution tempRes = new Resolution();
                tempRes.width = int.Parse(temp1[0]);
                tempRes.height = int.Parse(temp1[1]);
                tempRes.refreshRate = 60;
                resolutions[i] = tempRes;
                if (tempRes.Equals(preferedRes))
                {
                    resIndex = i;
                }
            }



        }
        else
        {
            setErrorFlag(0, true);
        }
    }

    private static void setErrorFlag(int index, bool value)
    {
        string temp = "";
        char[] charTemp = errorFlags.ToCharArray();
        for (int i = 0; i < errorFlags.Length; i++)
        {
            if (i == index)
            {
                if (value)
                {
                    temp += "1";
                }
                else
                {
                    temp += charTemp[i];
                }
            }
        }
        errorFlags = temp;
    }

    private static void resetFlags()
    {
        string temp = "";
        for (int i = 0; i < errorFlags.Length; i++)
        {
            temp += "0";
        }
        errorFlags = temp;
    }





    public static void switchResSetting(bool nextOrFalse)
    {
        if (nextOrFalse)
        {
            resIndex = (resIndex + 1) % resolutions.Length;
        }
        else
        {
            resIndex--;
            if (resIndex < 0)
            {
                resIndex = resolutions.Length - 1;
            }
        }
    }

    public static Resolution getCurRes()
    {
        return resolutions[resIndex];
    }

    


}
