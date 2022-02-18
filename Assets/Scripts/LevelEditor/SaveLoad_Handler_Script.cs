using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class SaveLoad_Handler_Script : MonoBehaviour
{
    [SerializeField] TMP_Dropdown mapSize;
    [SerializeField] TMP_InputField mapName;
    [SerializeField] TMP_Dropdown existingFileName;
    [SerializeField] TMP_InputField saveAsName;
    [SerializeField] ContentBar_Handler_Script contentBar;


    public void save()
    {
        StreamWriter sr = new StreamWriter(contentBar.MapFilePath + saveAsName.text + ".MapData", true);
        Debug.Log(contentBar.MapFilePath + saveAsName.text + ".MapData");
        if (existingFileName.value.Equals("New File"))
        {
            //create new file
            sr.WriteLine("hello");
        }
        else
        {
            //save to existing file
            sr.WriteLine("hello2");
        }
        sr.Flush();
    }

    public void load()
    {

    }


    
}
