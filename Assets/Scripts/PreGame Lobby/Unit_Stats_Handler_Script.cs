using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Stats_Handler_Script : MonoBehaviour
{
    [SerializeField] Unit_Stat_Tracker_Script[] trackers = new Unit_Stat_Tracker_Script[7];
    [SerializeField] GameObject Loadout;
    [SerializeField] GameObject Inventory;


    public void getStats(CharacterData currentlySelected)
    {
        trackers[0].sliderBar.value = currentlySelected.ActionPoints;
        trackers[0].textTag.text = "Action Points";

        trackers[1].sliderBar.value = currentlySelected.HealthPoints;
        trackers[1].textTag.text = "Health";

        trackers[2].sliderBar.value = currentlySelected.Strength;
        trackers[2].textTag.text = "Strength";

        trackers[3].sliderBar.value = currentlySelected.Accuracy;
        trackers[3].textTag.text = "Accuracy";

        trackers[4].sliderBar.value = currentlySelected.ReactionTime;
        trackers[4].textTag.text = "Reaction Time";

        trackers[5].sliderBar.value = currentlySelected.Dexterity;
        trackers[5].textTag.text = "Dexterity";

        trackers[6].sliderBar.value = currentlySelected.WeaponHandling;
        trackers[6].textTag.text = "Weapon Handling";

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
}
