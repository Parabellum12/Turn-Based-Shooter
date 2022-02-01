using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditor_Handler_Script : MonoBehaviour
{
    [SerializeField] World_Handler_Script worldDataHandler;
    [SerializeField] TileChooser_Handler_Script tileChooserHandler;
    [SerializeField] ContentBar_Handler_Script contentBarHandler;
    [SerializeField] Tools_Handler_Script toolsHandler;
    int UILayer;

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
    }

    void Update()
    {
       
        if (Input.GetMouseButton(0) && ValidMousePositionForTilePlacement())
        {
            //left click held
            HandleLeftClick();
        }
    }


    private void HandleLeftClick()
    {
        switch(toolsHandler.SelectedTool)
        {
            case Tools_Handler_Script.Tools.Brush:
                worldDataHandler.setTile(tileChooserHandler.CurrentSelectedTile, UtilClass.getMouseWorldPosition(), 0);
                break;
            case Tools_Handler_Script.Tools.Erase:
                worldDataHandler.setTile(null, UtilClass.getMouseWorldPosition(), 0);
                break;
        }
    }

    private bool ValidMousePositionForTilePlacement()
    {
        if (IsPointerOverUIElement() || worldDataHandler.getBuildLayers() == null || worldDataHandler.getBuildLayers().Count == 0)
        {
            return false;
        }
        return true;
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }


}
