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
        if (ValidMousePositionForTilePlacement())
        {
            //tile editor stuff 
            if (Input.GetMouseButton(0))
            {
                //left click down
                HandleLeftClick();
            }
            if (Input.GetMouseButtonUp(0))
            {
                //left click release
                HandleLeftClickRelease();
            }
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
    Vector2Int boxFillOriginXY = new Vector2Int(-1,-1);
    bool validBoxFillStart = true;
    Vector2Int boxFillLastGridPos = new Vector2Int(-1,-1);
    Dictionary<Vector2Int, World_Handler_Script.worldBuildTileTransferData> BoxFillLastTiles = new Dictionary<Vector2Int, World_Handler_Script.worldBuildTileTransferData>();
    private void handleBoxFillRequest(bool starting_ending)
    {
        //Debug.Log("HandleBoxFill: " + starting_ending);
        if (starting_ending)
        {
            Vector2 nullTest = new Vector2(-1, -1);
            if (boxFillOriginXY == nullTest)
            {
                worldDataHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int x, out int y);
                if (!worldDataHandler.getBuildLayers().inBounds(x, y))
                {
                    validBoxFillStart = false;
                }
                boxFillOriginXY = new Vector2Int(x, y);
                boxFillLastGridPos = new Vector2Int(x, y);
            }
            else
            {
                //Debug.Log("Fill: " + validBoxFillStart);
                if (validBoxFillStart)
                {
                    worldDataHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int x, out int y);
                    if (x == boxFillLastGridPos.x && y == boxFillLastGridPos.y)
                    {
                        //no new box
                        return;
                    }

                    foreach (KeyValuePair<Vector2Int, World_Handler_Script.worldBuildTileTransferData> entry in BoxFillLastTiles)
                    {
                        if (!BoxFillTileInBounds(boxFillOriginXY, new Vector2Int(x,y), entry.Key.x, entry.Key.y))
                        {
                            worldDataHandler.setTile(entry.Value, entry.Key, 0);
                            //Debug.Log("return To Original: " + entry.Key.ToString() + " Value:" + entry.Value.ToString());
                        }
                    }

                    Vector2Int xMinToMax = returnMinToMax((int)boxFillOriginXY.x, x);
                    Vector2Int YMinToMax = returnMinToMax((int)boxFillOriginXY.y, y);
                    //Debug.Log("origin:" + boxFillOriginXY.ToString() + " : new:" + x + "," + y + " :: XMinToMax:" + xMinToMax.ToString() + " ; YMinToMax:" + YMinToMax.ToString());
                    for (int i = xMinToMax.x; i <= xMinToMax.y; i++)
                    {
                        for (int j = YMinToMax.x; j <= YMinToMax.y; j++)
                        {
                            if (worldDataHandler.getBuildLayers().inBounds(i,j))
                            {
                                if (!BoxFillLastTiles.ContainsKey(new Vector2Int(i, j)) && !BoxFillTileInBounds(boxFillOriginXY, boxFillLastGridPos, i, j))
                                {
                                    BoxFillLastTiles.Add(new Vector2Int(i, j), worldDataHandler.getBuildLayers().getGridObject(i, j).getTransferData());
                                }
                                boxFillSetterHandler(i, j);
                            }
                        }
                    }

                    boxFillLastGridPos = new Vector2Int(x, y);

                }
            }
        }
        else
        {
            validBoxFillStart = true;
            boxFillOriginXY = new Vector2Int(-1, -1);
            boxFillLastGridPos = new Vector2Int(-1, -1);
            BoxFillLastTiles.Clear();
        }
    }

    private bool BoxFillTileInBounds(Vector2Int origin, Vector2Int current, int x, int y)
    {
        Vector2Int xMinToMax = returnMinToMax(origin.x, current.x);
        Vector2Int yMinToMax = returnMinToMax(origin.y, current.y);
        if (x < xMinToMax.x || x > xMinToMax.y
            || y < yMinToMax.x || y > yMinToMax.y)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void boxFillSetterHandler(int i, int j)
    {
        //setter
        //Debug.Log("InBounds!:" + i + "," + j);
        worldDataHandler.setTile(tileChooserHandler.CurrentSelectedTile, new Vector2Int(i, j), 0);

    }

    private Vector2Int returnMinToMax(int x, int y)
    {
        if (x < y)
        {
            return new Vector2Int(x, y);
        }
        else
        {
            return new Vector2Int(y, x);
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
