using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ContentBar_Handler_Script : MonoBehaviour
{
    [SerializeField] GameObject Settings;
    [SerializeField] GameObject SaveLoad;
    [SerializeField] GameObject WorldTiles;
    public RectTransform localTransform;
    string fileSystemSeperator;
    public string MapFilePath;

    private void Start()
    {
        toSettings();
        DealWithFileSelectionSettings();
        if (Application.streamingAssetsPath.Contains("/"))
        {
            fileSystemSeperator = "/";
        }
        else if (Application.streamingAssetsPath.Contains("\\"))
        {
            fileSystemSeperator = "\\";
        }
        MapFilePath = Application.streamingAssetsPath + fileSystemSeperator + "Maps" + fileSystemSeperator;
    }

    public void toSettings()
    {
        setAllInactive();
        Settings.SetActive(true);
    }

    public void toSaveLoad()
    {
        setAllInactive();
        updattMapFileList();
        SaveLoad.SetActive(true);
    }

    public void toWorldBuilding()
    {
        setAllInactive();
        WorldTiles.SetActive(true);
    }

    private void setAllInactive()
    {
        Settings.SetActive(false);
        SaveLoad.SetActive(false);
        WorldTiles.SetActive(false);
    }





    // save/load
    [SerializeField] TMP_Dropdown LoadFileDropdown;
    [SerializeField]Dictionary<int, string> FilDropDown_ValueToFileName = new Dictionary<int, string>();

    public string getLoadFile()
    {
        Debug.Log("get load file");
        return LoadFileDropdown.options[LoadFileDropdown.value].text;
    }
    private void updattMapFileList()
    {
        LoadFileDropdown.options.Clear();
        //LoadFileDropdown.ClearOptions();
        FilDropDown_ValueToFileName.Clear();
        string[] files = System.IO.Directory.GetFiles(Application.streamingAssetsPath + fileSystemSeperator + "Maps", "*.MapData");
        //Debug.Log(Application.streamingAssetsPath + fileSystemSeperator + "Maps");
        //Debug.Log("Files: Length:" + files.Length);
        TMP_Dropdown.OptionData customOption = new TMP_Dropdown.OptionData("New File");
        LoadFileDropdown.options.Add(customOption);
        for (int i = 0; i < files.Length; i++)
        {
            FilDropDown_ValueToFileName.Add(i+1, files[i]);
            //Debug.Log(files[i]);
            string name = "";
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
                    name = temp[j] + name;
                }
                else
                {
                    if (temp[j] == '.')
                    {
                        fileName = true;
                    }
                }
            }



            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(name);
            LoadFileDropdown.options.Add(optionData);
        }
    }

    [SerializeField] TMP_InputField saveFileName;
    [SerializeField] Button loadButton;
    public void DealWithFileSelectionSettings()
    {
        if (LoadFileDropdown.value == 0)
        {
            saveFileName.interactable = true;
            loadButton.interactable = false;
        }
        else
        {
            saveFileName.interactable = false;
            loadButton.interactable = true;
        }
    }
}
