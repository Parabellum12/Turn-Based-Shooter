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
        if (Input.GetMouseButtonUp(0))
        {
            //end left click hold
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
            case Tools_Handler_Script.Tools.BoxFill:
                handleBoxFillRequest(true);
                break;

        }
    }

    private void HandleLeftClickRelease()
    {
        switch (toolsHandler.SelectedTool)
        {
            case Tools_Handler_Script.Tools.BoxFill:
                handleBoxFillRequest(false);
                break;
        }
    }


    //boxfill
    Vector2 originXY = new Vector2(-1,-1);
    bool validStart = true;
    private void handleBoxFillRequest(bool starting_ending)
    {
        if (starting_ending)
        {
            Vector2 nullTest = new Vector2(-1, -1);
            if (originXY == nullTest)
            {
                worldDataHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int x, out int y);
                if (!worldDataHandler.getBuildLayers().inBounds(x, y))
                {
                    validStart = false;
                }
                Vector2 coords = new Vector2(x, y);
            }
        }
        else
        {
            if (validStart)
            {
                worldDataHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int endingX, out int endingY);
                Vector2 xMinToMax = returnMinToMax(Mathf.RoundToInt(originXY.x), endingX);
                Vector2 yMinToMax = returnMinToMax(Mathf.RoundToInt(originXY.y), endingY);
            }
            else
            {
                validStart = true;
                originXY = new Vector2(-1, -1);
            }
        }
    }

    private Vector2 returnMinToMax(int x, int y)
    {
        if (x < y)
        {
            return new Vector2(x, y);
        }
        else
        {
            return new Vector2(y, x);
        }
    }

    private bool ValidMousePositionForTilePlacement()
    {
        if (IsPointerOverUIElement() || worldDataHandler.getBuildLayers() == null)
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
