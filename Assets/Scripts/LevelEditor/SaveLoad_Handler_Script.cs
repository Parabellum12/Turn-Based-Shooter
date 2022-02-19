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
        //format 
        /*
         * map size x, y, map name
         * TransDat:x,y,TBD:
         * Tile 2: transfer data
         * 
         */
        StreamWriter sr = new StreamWriter(contentBar.MapFilePath + saveAsName.text + ".MapData", true);
        Debug.Log(contentBar.MapFilePath + saveAsName.text + ".MapData");
        sr.WriteLine("hello");
        sr.Flush();
    }

    public void load()
    {

    }


    
}
