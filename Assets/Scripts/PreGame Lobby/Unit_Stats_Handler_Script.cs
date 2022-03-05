using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Unit_Stats_Handler_Script : MonoBehaviour
{
    [SerializeField] Unit_Stat_Tracker_Script[] trackers = new Unit_Stat_Tracker_Script[7];
    [SerializeField] GameObject Loadout;
    [SerializeField] GameObject Inventory;
    [SerializeField] TMP_Text unitClass;

    private CharacterData currentSelected;
    public void getStats(CharacterData currentlySelected)
    {
        currentSelected = currentlySelected;

        currentSelected.updateStats();
        updateUnitClassText();

    }

    public void updateStatText()
    {
        trackers[0].sliderBar.value = currentSelected.Armor;
        trackers[0].textTag.text = "Armor";

        trackers[1].sliderBar.value = currentSelected.ActionPoints;
        trackers[1].textTag.text = "Action Points";

        trackers[2].sliderBar.value = currentSelected.HealthPoints;
        trackers[2].textTag.text = "Health";

        trackers[3].sliderBar.value = currentSelected.Strength;
        trackers[3].textTag.text = "Strength";

        trackers[4].sliderBar.value = currentSelected.Accuracy;
        trackers[4].textTag.text = "Accuracy";

        trackers[5].sliderBar.value = currentSelected.ReactionTime;
        trackers[5].textTag.text = "Reaction Time";

        trackers[6].sliderBar.value = currentSelected.Dexterity;
        trackers[6].textTag.text = "Dexterity";

        trackers[7].sliderBar.value = currentSelected.WeaponHandling;
        trackers[7].textTag.text = "Weapon Handling";

    }

    public void switchToInventory()
    {
        Loadout.SetActive(false);
        Inventory.SetActive(true);
    }

    public void switchToLoadout()
    {
        Loadout.SetActive(true);
        Inventory.SetActive(false);
    }

    public void updateUnitClassText()
    {
        currentSelected.updateStats();
        unitClass.text = currentSelected.characterClass.ToString();
        updateStatText();
    }

    public void nextUnitClass()
    {
        if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Defender)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Attacker;
        }
        else if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Attacker)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Engineer;

        }
        else if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Engineer)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Ranger;

        }
        else if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Ranger)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Defender;

        }
        updateUnitClassText();
    }

    public void previousUnitClass()
    {
        if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Defender)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Ranger;
        }
        else if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Attacker)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Defender;

        }
        else if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Engineer)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Attacker;

        }
        else if (currentSelected.characterClass == CharacterData.CharacterClassEnum.Ranger)
        {
            currentSelected.characterClass = CharacterData.CharacterClassEnum.Engineer;

        }
        updateUnitClassText();
    }
}
