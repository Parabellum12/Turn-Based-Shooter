using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools_Handler_Script : MonoBehaviour
{
    [SerializeField] Tool_Selector_Script Select;
    [SerializeField] Tool_Selector_Script Place;
    [SerializeField] Tool_Selector_Script Erase;
    [SerializeField] Tool_Selector_Script BoxFill;


    public enum Tools
    {
        Select,
        Brush,
        Erase,
        BoxFill
    };

    public Tools SelectedTool = new Tools();
    // Start is called before the first frame update
    void Start()
    {
        ToolSelect();

    }

    public void ToolSelect()
    {
        DeSelectAll();
        Select.Select();
        SelectedTool = Tools.Select;
    }

    public void ToolPlace()
    {
        DeSelectAll();
        Place.Select();
        SelectedTool = Tools.Brush;
    }

    public void toolErase()
    {
        DeSelectAll();
        Erase.Select();
        SelectedTool = Tools.Erase;
    }

    public void toolBoxFill()
    {

        DeSelectAll();
        BoxFill.Select();
        SelectedTool = Tools.BoxFill;
    }

    private void DeSelectAll()
    {
        Select.DeSelect();
        Place.DeSelect();
        Erase.DeSelect();
        BoxFill.DeSelect();
    }
}
