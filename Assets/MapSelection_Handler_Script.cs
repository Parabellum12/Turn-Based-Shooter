using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelection_Handler_Script : MonoBehaviour
{
    GameObject mapSelectorPrefab;
    string[] mapFiles;


    public void Start()
    {
        mapFiles = getMapFiles();
    }


    private string[] getMapFiles()
    {
        return null;
    }
}
