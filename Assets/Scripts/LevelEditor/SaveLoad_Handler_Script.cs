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


    public void save()
    {
        if (existingFileName.value.Equals("New File"))
        {
            //create new file
        }
        else
        {
            //save to existing file
        }
    }

    public void load()
    {

    }


    
}
