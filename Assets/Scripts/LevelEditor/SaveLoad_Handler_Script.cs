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
           // Debug.Log(contentBar.MapFilePath + saveAsName.text + ".MapData");
        }
        else
        {
            sr = new StreamWriter(contentBar.MapFilePath + existingFileName.options[existingFileName.value].text + ".MapData", false);
            //Debug.Log(contentBar.MapFilePath + existingFileName.options[existingFileName.value].text + ".MapData");
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
        sr.Close();
    }

    public void load(string fileName)
    {
        string filePath = worldHandler.MapFolderFilePath + fileName + ".MapData";
        //Debug.Log("Load File:" + fileName);
        //Debug.Log("Load File Path:" + filePath);
        StreamReader sr = new StreamReader(filePath);
        string fileContents = sr.ReadToEnd();
       // Debug.Log("Reading:" + fileContents);
        //Debug.Log(fileContents.Split('\n').Length);
        string[] contentsPerLine = fileContents.Split('\n');
        worldHandler.genNewMap(int.Parse(contentsPerLine[0].Split(',')[0]), int.Parse(contentsPerLine[0].Split(',')[1]));
        for (int i = 1; i < contentsPerLine.Length-1; i++)
        {
            World_Handler_Script.worldBuildTileTransferData transDat = new World_Handler_Script.worldBuildTileTransferData();
            if (!transDat.setDataFromSaveString(contentsPerLine[i]))
            {
                Debug.LogError("Load File Fail: \n\tIncorrect Data Type: " + contentsPerLine[i]);
            }
            worldHandler.setTile(transDat, new Vector2Int(transDat.x, transDat.y), 0);
        }

        sr.Close();
    }


    
}
