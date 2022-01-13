using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings_Handler
{
    public static Resolution[] resolutions;
    public static int resIndex = 0;
    public static Resolution preferedRes;

    public static FullScreenMode[] fullScreenModes = new FullScreenMode[]
    {
        FullScreenMode.FullScreenWindow,
        FullScreenMode.MaximizedWindow,
        FullScreenMode.Windowed,
    };
    public static FullScreenMode preferedScreenMode;

    public static int fullSceenModesIndex = 0;




    static string errorFlags = "00";
    static Dictionary<int, string> errorIndexToKey = new Dictionary<int, string>()
    {
        {0, "Resolution"},
        {1, "WindowMode"}
    };
    /*resolution
     * 
     * 
     * 
     */

    public static void resetSettings()
    {
        for (int i = 0; i < errorFlags.Length; i++)
        {
            setErrorFlag(i, true);
        }
        checkAndFixErrors();
        setPreferences();
    }


    public static void loadSettings()
    {
        resetFlags();
        loadResolutionSettings();
        loadFullscreenWindowSettings();



        checkAndFixErrors();
        setPreferences();

    }


    private static void checkAndFixErrors()
    {
        char[] error_flags = errorFlags.ToCharArray();
        bool errorFix = false;
        for (int i = 0; i < error_flags.Length; i++)
        {
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
                    case "WindowMode":
                        fixFullscreenWindowSettings();
                        loadFullscreenWindowSettings();
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
        Resolution[] arr = uniqueRes(Screen.resolutions);
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

    private static Resolution[] uniqueRes(Resolution[] resArr)
    {
        List<Resolution> temp = new List<Resolution>();
        List<string> tempString = new List<string>();
        for (int i = 0; i < resArr.Length; i++)
        {
            string value = resArr[i].width + ":" + resArr[i].height;
            if (!tempString.Contains(value))
            {
                tempString.Add(value);
                temp.Add(resArr[i]);
            }
        }
        return temp.ToArray();
    }





    private static void loadResolutionSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionNumber"))
        {
            setErrorFlag(0, false);

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

    private static void fixFullscreenWindowSettings()
    {
        PlayerPrefs.SetInt("WindowMode", 0);
    }

    private static void loadFullscreenWindowSettings()
    {
        if (PlayerPrefs.HasKey("WindowMode"))
        {
            fullSceenModesIndex = PlayerPrefs.GetInt("WindowMode");
            preferedScreenMode = fullScreenModes[fullSceenModesIndex];
        }
        else
        {
            setErrorFlag(1, true);
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


    public static void switchWindowMode(bool nextOrLast)
    {
        if (nextOrLast)
        {
            //next
            fullSceenModesIndex = (fullSceenModesIndex + 1) % 3;
        }
        else
        {
            //last
            fullSceenModesIndex--;
            if (fullSceenModesIndex < 0)
            {
                fullSceenModesIndex = 2;
            }
        }
    }

    public static FullScreenMode getCurWindowMode()
    {
        return fullScreenModes[fullSceenModesIndex];
    }

    


}
