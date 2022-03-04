using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public string CharacterName;

    public WeaponItemData primary_Weapon = null;
    public WeaponItemData secondary_Weapon = null;

    public int Armor;
    //0-100; 50 = no debuff or buff; < 50 = debuff; 50 < = buff checked by 10s or 5s
    public int ActionPoints;
    public int Strength;
    public int Accuracy;
    public int HealthPoints;
    public int ReactionTime;
    public int Dexterity;
    public int WeaponHandling;

    static bool randomizeStats = false;

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
        switch (characterClass)
        {
            case CharacterClassEnum.Defender:
                break;
            case CharacterClassEnum.Attacker:
                break;
            case CharacterClassEnum.Engineer:
                break;
            case CharacterClassEnum.Ranger:
                break;
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
  