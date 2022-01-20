using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public string CharacterName;

    public WeaponItemData primary_Weapon = null;
    public WeaponItemData secondary_Weapon = null;

    //0-100; 50 = no debuff or buff; < 50 = debuff; 50 < = buff checked by 10s or 5s
    public int ActionPoints;
    public int Strength;
    public int Accuracy;
    public int HealthPoints;
    public int ReactionTime;
    public int Dexterity;
    public int WeaponHandling;

    static bool randomizeStats = false;

    public Sprite ClassImage;

    public CharacterData(bool random)
    {
        if (!random)
        {
            ActionPoints = 50;
            Strength = 50;
            Accuracy = 50;
            HealthPoints = 50;
            ReactionTime = 50;
            Dexterity = 50;
            WeaponHandling = 50;
        }
        else
        {
            ActionPoints = getAverageRandom();
            Strength = getAverageRandom();
            Accuracy = getAverageRandom();
            HealthPoints = getAverageRandom();
            ReactionTime = getAverageRandom();
            Dexterity = getAverageRandom();
            WeaponHandling = getAverageRandom();
        }

        CharacterName = Character_Name_Handler.generateName();
    }

    private int getAverageRandom()
    {
        int total = 0;
        for (int i = 0; i < 5; i++)
        {
            total += Random.Range(20, 80);
        }
        return total / 5;
    }

}
  