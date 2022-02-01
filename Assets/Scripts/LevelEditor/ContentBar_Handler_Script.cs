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


    private void Start()
    {
        toSettings();
        DealWithFileSelectionSettings();
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
    private void updattMapFileList()
    {
        LoadFileDropdown.options.Clear();
        string[] files = System.IO.Directory.GetFiles(Application.streamingAssetsPath + "\\Maps", "*.MapData");

        TMP_Dropdown.OptionData customOption = new TMP_Dropdown.OptionData("New File");
        LoadFileDropdown.options.Add(customOption);
        for (int i = 0; i < files.Length; i++)
        {
            //Debug.Log(sr.ReadToEnd());
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
