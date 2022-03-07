using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public string CharacterName;

    public WeaponItemData Weapon = null;

    public int Armor;
    //0-100; 50 = no debuff or buff; < 50 = debuff; 50 < = buff checked by 10s or 5s
    public int ActionPoints;
    public int Strength;
    public int Accuracy;
    public int HealthPoints;
    public int ReactionTime;
    public int Dexterity;
    public int WeaponHandling;


    //defender: high health/armor clsoe range, shotgun
    //attacker: medium health/armor rifle
    //engineer: low health/armor, can heal/repair others, pistol
    //ranger: medium health, low armor, sniper
    public enum CharacterClassEnum
    {
        Defender,
        Attacker,
        Engineer,
        Ranger,
    };

    public Sprite ClassImage;
    public CharacterClassEnum characterClass = CharacterClassEnum.Attacker;

    public CharacterData(bool random)
    {

        updateStats();
        CharacterName = Character_Name_Handler.generateName();
    }

    public void updateStats()
    {
        switch (characterClass)
        {
            case CharacterClassEnum.Defender:
                setDataValues(100, 25, 30, 100, 60, 20, 35, 50);
                break;
            case CharacterClassEnum.Attacker:
                setDataValues(75, 75, 50, 75, 70, 40, 50, 50);
                break;
            case CharacterClassEnum.Engineer:
                setDataValues(50, 50, 40, 40, 20, 90, 20, 50);
                break;
            case CharacterClassEnum.Ranger:
                setDataValues(25, 100, 100, 60, 90, 50, 90, 50);
                break;
        }
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

    private void setDataValues(int armor, int ap, int acc, int hp, int rt, int dex, int weaHandle, int str)
    {
        this.Armor = armor;
        ActionPoints = ap;
        Accuracy = acc;
        HealthPoints = hp;
        ReactionTime = rt;
        Dexterity = dex;
        WeaponHandling = weaHandle;
        Strength = str;
    }

}
  