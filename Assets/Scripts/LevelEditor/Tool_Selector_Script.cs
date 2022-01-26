using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tool_Selector_Script : MonoBehaviour
{
    [SerializeField] Image selectedIndicator;
    bool selected = false;


    public void DeSelect()
    {
        selectedIndicator.enabled = false;
        selected = false;
    }

    public void Select()
    {
        selectedIndicator.enabled = true;
        selected = true;
    }
}
