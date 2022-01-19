using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class Character_Name_Handler
{
    public static string[] firstNames = null;
    public static string[] lastNames = null;
    private static int seed = 0;

    public static void setupCharacterNameList()
    {
        string pathS = Application.dataPath + "/TextFiles/FirstNames.txt";
        string pathSLast = Application.dataPath + "/TextFiles/LastNames.txt";
        StreamReader reader = new StreamReader(pathS, true);
        string fullText = reader.ReadToEnd();
        string[] StringArray = fullText.Split('\n');
        firstNames = StringArray;


        reader = new StreamReader(pathSLast, true);
        fullText = reader.ReadToEnd();
        StringArray = fullText.Split('\n');
        lastNames = StringArray;
    }


    
    public static string generateName()
    {
        if (seed == 0)
        {
            seed = Mathf.RoundToInt(Random.value * 1000);
        }
        //Debug.Log("generateName Ran " + seed + " Times");
        if (firstNames == null || lastNames == null || firstNames.Length == 0 || lastNames.Length == 0)
        {
            setupCharacterNameList();
        }
        //Random.seed = seed;
        seed++;
        Random.InitState(seed);
        ;
        int index = Mathf.FloorToInt(Random.Range(0, firstNames.Length));

        Random.Range(0, lastNames.Length);
        int indexLast = Mathf.FloorToInt(Random.Range(0, lastNames.Length));

        string whyyyyy = firstNames[index].ToString() + "_" + lastNames[indexLast];
        whyyyyy = whyyyyy.Trim();
        char[] temp = whyyyyy.ToCharArray();
        string valid = "abcdefghijklmnopqrstuvwxyz";
        string validUpper = valid.ToUpper();
        string returner = "";
        for (int i = 0; i < temp.Length; i++)
        {
            if (valid.Contains(temp[i].ToString()) || validUpper.Contains(temp[i].ToString()))
            {
                returner += temp[i];
                //Debug.Log(temp[i]);
            }
            else
            {
                if (temp[i] == '_')
                {
                    returner += ' ';
                    //Debug.Log("Empty");
                }
            }
        }
        return returner;
    }

}
