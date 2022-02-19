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
    [SerializeField] World_Handler_Script worldHandler;


    public void save()
    {
        //format 
        /*
         * map size x, y, map name
         * TransDat:x,y,TBD:
         * Tile 2: transfer data
         * 
         */
        if ((worldHandler.getBuildLayers() == null || saveAsName.text == null || saveAsName.text.Length == 0) && existingFileName.value == 0)
        {
            return;
        }
        StreamWriter sr;
        if (existingFileName.value == 0)
        {
            sr = new StreamWriter(contentBar.MapFilePath + saveAsName.text + ".MapData", false);
            Debug.Log(contentBar.MapFilePath + saveAsName.text + ".MapData");
        }
        else
        {
            sr = new StreamWriter(contentBar.MapFilePath + existingFileName.options[existingFileName.value].text + ".MapData", false);
            Debug.Log(contentBar.MapFilePath + existingFileName.options[existingFileName.value].text + ".MapData");
        }
        sr.WriteLine(worldHandler.getBuildLayers().width + "," + 
            worldHandler.getBuildLayers().height + "," + 
            mapName.text);
        for (int i = 0; i < worldHandler.getBuildLayers().width; i++ )
        {
            for (int j = 0; j < worldHandler.getBuildLayers().height; j++)
            {
                sr.WriteLine(worldHandler.getBuildLayers().getGridObject(i,j).getTransferData().getDataAsSaveString());
            }
        }
        sr.Flush();
    }

    public void load()
    {

    }


    
}
