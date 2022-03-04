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

        trackers[0].sliderBar.value = currentlySelected.Armor;
        trackers[0].textTag.text = "Armor";

        trackers[1].sliderBar.value = currentlySelected.ActionPoints;
        trackers[1].textTag.text = "Action Points";

        trackers[2].sliderBar.value = currentlySelected.HealthPoints;
        trackers[2].textTag.text = "Health";

        trackers[3].sliderBar.value = currentlySelected.Strength;
        trackers[3].textTag.text = "Strength";

        trackers[4].sliderBar.value = currentlySelected.Accuracy;
        trackers[4].textTag.text = "Accuracy";

        trackers[5].sliderBar.value = currentlySelected.ReactionTime;
        trackers[5].textTag.text = "Reaction Time";

        trackers[6].sliderBar.value = currentlySelected.Dexterity;
        trackers[6].textTag.text = "Dexterity";

        trackers[7].sliderBar.value = currentlySelected.WeaponHandling;
        trackers[7].textTag.text = "Weapon Handling";

        updateUnitClassText();

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
        unitClass.text = currentSelected.characterClass.ToString();
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
