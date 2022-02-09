using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileChooser_Handler_Script : MonoBehaviour
{
    [SerializeField] Image openCloseArrow;
    [SerializeField] TMP_Text assetClassTypeLabel;
    [SerializeField] RectTransform AssetPickerTopMargin;
    float openPos;
    float closePos;
    float timeToMove = .1f;
    [SerializeField] RectTransform localTransform;
    private float moveDist;
    // Start is called before the first frame update
    void Start()
    {
        closePos = localTransform.localPosition.x;
        openPos = closePos + localTransform.rect.width;
        moveDist = Mathf.Abs(openPos - closePos);
        //Debug.Log(closePos + ";" + openPos);
        close();
        setSelectedTileType();
    }

    public void openOrClose()
    {
        //Debug.Log("move called:" + localTransform.localPosition.x);
        if (localTransform.localPosition.x > openPos - 1 && localTransform.localPosition.x < openPos + 1)
        {
            //is open
            close();
        }
        else if (localTransform.localPosition.x > closePos - 1 && localTransform.localPosition.x < closePos + 1)
        {
            //is closed
            open();
        }
    }

    public void open()
    {
        //Debug.Log("open called");
        move = true;
        openClose = true;
    }

    public void close()
    {
        //Debug.Log("close called");
        move = true;
        openClose = false;
    }

    bool move = false;
    bool openClose = false;

    public void Update()
    {
        if (move)
        {
            if (openClose)
            {
                //open
                if (localTransform.localPosition.x > openPos)
                {
                    Vector2 posreturn = localTransform.localPosition;
                    posreturn.x = openPos;
                    //Debug.Log("open ended");
                    localTransform.localPosition = posreturn;
                    move = false;
                    openClose = false;
                    openCloseArrow.transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else
                {
                    Vector2 pos = localTransform.localPosition;
                    pos.x += (moveDist / timeToMove) * Time.deltaTime;
                    localTransform.localPosition = pos;
                }
            }
            else
            {
                //close
                if (localTransform.localPosition.x < closePos)
                {
                    Vector2 posreturn = localTransform.localPosition;
                    posreturn.x = closePos;
                    //Debug.Log("close ended");
                    localTransform.localPosition = posreturn;
                    move = false;
                    openClose = false;
                    openCloseArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    Vector2 pos = localTransform.localPosition;
                    pos.x -= (moveDist / timeToMove) * Time.deltaTime;
                    localTransform.localPosition = pos;
                }
            }
        }
    }



    //tile Asset Selection Handling

    [SerializeField] GameObject TileSelectorPrefab;

    [SerializeField] TileBuildData[] GroundTiles;
    private TileBuildData[] getSelectedTileType()
    {
        switch(assetClassTypeLabel.text)
        {
            case "Ground":
                return GroundTiles;
            default:
                Debug.LogError("Tile Type Not Found");               
                return null;
        };
    }

    private Vector2 GetPositionToSet(int index)
    {
        Vector2 returner = Vector2.zero;
        returner.x = index % 3;
        while (index > 3)
        {
            returner.y++;
            index -= 3;
        }
        return returner;
    }
    List<TileAssetSelector_Handelr_Script> CurrentlyVisibleTileAssets = new List<TileAssetSelector_Handelr_Script>();
    public void setSelectedTileType()
    {
        foreach (TileAssetSelector_Handelr_Script scr in CurrentlyVisibleTileAssets)
        {
            scr.DeleteMe();
        }
        TileBuildData[] CurrentSelectedTileType = getSelectedTileType();
        Vector2 topLeftCorner = localTransform.localPosition;
        topLeftCorner.x -= localTransform.rect.width / 2;
        topLeftCorner.y += (localTransform.rect.height / 2) - (AssetPickerTopMargin.rect.height * 2);
        float[] gridPosValues = {
           0f - (localTransform.rect.width / 2f),
           0f,
           (localTransform.rect.width / 2f)
        };
        for (int i = 0; i < CurrentSelectedTileType.Length; i++)
        {
            Vector2 gridPos = GetPositionToSet(i);
            GameObject temp = Instantiate(TileSelectorPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            TileAssetSelector_Handelr_Script tempScript = temp.GetComponent<TileAssetSelector_Handelr_Script>();
            tempScript.AssetSelectRectTransform.localPosition = new Vector2(gridPos.x, gridPos.y);
            CurrentlyVisibleTileAssets.Add(tempScript);
            
        }
    }



}
