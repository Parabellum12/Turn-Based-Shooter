using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools_Handler_Script : MonoBehaviour
{
    [SerializeField] Tool_Selector_Script Select;
    [SerializeField] Tool_Selector_Script Place;
    // Start is called before the first frame update
    void Start()
    {
        ToolSelect();

    }

    public void ToolSelect()
    {
        DeSelectAll();
        Select.Select();
    }

    public void ToolPlace()
    {
        DeSelectAll();
        Place.Select();
    }

    private void DeSelectAll()
    {
        Select.DeSelect();
        Place.DeSelect();
    }
}
