using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SessionPersistantDataHandler
{
    static string plrName = "";
    static List<CharacterData> persistantUnitSquad = new List<CharacterData>();
    
    public static void setPlrName(string name)
    {
        plrName = name;
    }
    public static string getPlrName()
    {
        return plrName;
    }

    public static void setPersistantUnitSquad(List<CharacterData> dat)
    {
        Debug.Log("set character data");
        persistantUnitSquad.Clear();
        persistantUnitSquad = dat;
    }
    public static void setPersistantUnitSquad(CharacterData[] dat)
    {
        Debug.Log("set character data");
        persistantUnitSquad.Clear();
        persistantUnitSquad.AddRange(dat);
    }

    public static List<CharacterData> getPersistantUnitData()
    {
        Debug.Log("get character data");
        return persistantUnitSquad;
    }

}
