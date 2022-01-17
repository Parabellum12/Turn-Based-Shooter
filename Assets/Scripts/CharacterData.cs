using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    string CharacterName;

    WeaponItemData primary_Weapon = null;
    WeaponItemData secondary_Weapon = null;

    //0-100; 50 = no debuff or buff; < 50 = debuff; 50 < = buff checked by 10s or 5s
    int ActionPoints = 50;
    int Strength = 50;
    int Accuracy = 50;
    int HealthPoints = 50;
    int ReactionTime = 50;
    int Dexterity = 50;
    int WeaponHandling = 50;

    static bool randomizeStats = false;


}
  